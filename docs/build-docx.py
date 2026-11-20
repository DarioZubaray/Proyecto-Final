import copy
import os
import re
import struct

import docx
from docx.shared import Inches, Pt
from docx.oxml.ns import qn
from docx.oxml import OxmlElement
from docx.text.paragraph import Paragraph

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DOCX_PATH = os.path.join(REPO, "docs", "TP Final - Analista Programador - Dario Zubaray.docx")
MD_DIR = os.path.join(REPO, "docs", "md")
MER_DIR = os.path.join(REPO, "docs", "mermaid-live")

PORTRAIT_W, PORTRAIT_H = 6.4, 9.4
LAND_W, LAND_H = 15.0, 8.7

doc = docx.Document(DOCX_PATH)
body = doc.element.body

sample_tblpr = None


def normalize_sectprs():
    sects = [body.find(qn("w:sectPr"))]
    for child in body.iterchildren():
        if child.tag == qn("w:p"):
            pPr = child.find(qn("w:pPr"))
            if pPr is not None:
                sp = pPr.find(qn("w:sectPr"))
                if sp is not None:
                    sects.append(sp)
    for sp in sects:
        if sp is None:
            continue
        for tag in ("w:pgSz", "w:pgMar"):
            el = sp.find(qn(tag))
            if el is None:
                continue
            for k, v in list(el.attrib.items()):
                try:
                    el.set(k, str(int(round(float(v)))))
                except ValueError:
                    pass


def heading_paras():
    out = []
    for child in body.iterchildren():
        if child.tag == qn("w:p"):
            p = Paragraph(child, doc)
            out.append((child, p.text.strip(), p.style.name))
    return out


def find_heading(text, style, exactly=False):
    for el, t, s in heading_paras():
        if s == style and (t == text if exactly else t.startswith(text)):
            return el
    raise SystemExit("heading not found: %r (%s)" % (text, style))


def remove_range(start_el, end_el):
    cur = start_el
    while cur is not None and cur is not end_el:
        nxt = cur.getnext()
        body.remove(cur)
        cur = nxt


def renumber_heading(el, new_text):
    p = Paragraph(el, doc)
    pPr = p._p.find(qn("w:pPr"))
    if pPr is not None:
        numPr = pPr.find(qn("w:numPr"))
        if numPr is not None:
            pPr.remove(numPr)
    for r in list(p._p.findall(qn("w:r"))):
        p._p.remove(r)
    p.add_run(new_text)


class Builder:
    def __init__(self):
        self.els = []

    @staticmethod
    def _tight(p):
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(0)
        return p

    def para(self, text="", style=None, tight=False):
        p = doc.add_paragraph(style=style) if style else doc.add_paragraph()
        if text:
            p.add_run(text)
        if tight:
            self._tight(p)
        self.els.append(p._p)
        return p

    def image(self, path, maxw, maxh, tight=False):
        p = doc.add_paragraph()
        p.add_run().add_picture(path, width=fit_width(path, maxw, maxh))
        if tight:
            self._tight(p)
        self.els.append(p._p)
        return p

    def table(self, rows_data):
        nrows = len(rows_data)
        ncols = max(len(r) for r in rows_data)
        t = doc.add_table(rows=nrows, cols=ncols)
        global sample_tblpr
        if sample_tblpr is not None:
            cur = t._tbl.find(qn("w:tblPr"))
            if cur is not None:
                t._tbl.remove(cur)
            t._tbl.insert(0, copy.deepcopy(sample_tblpr))
        for ri, row in enumerate(rows_data):
            for ci in range(ncols):
                cell = t.cell(ri, ci)
                txt = row[ci] if ci < len(row) else ""
                p = cell.paragraphs[0]
                add_runs(p, txt)
                if ri == 0:
                    for run in p.runs:
                        run.bold = True
        self.els.append(t._tbl)
        return t

    def section_break(self, landscape):
        p = self._tight(doc.add_paragraph())
        pPr = p._p.get_or_add_pPr()
        sect = copy.deepcopy(body.find(qn("w:sectPr")))
        if landscape:
            pg = sect.find(qn("w:pgSz"))
            pg.set(qn("w:w"), "24480")
            pg.set(qn("w:h"), "15840")
            pg.set(qn("w:orient"), "landscape")
            mar = sect.find(qn("w:pgMar"))
            if mar is not None:
                mar.set(qn("w:left"), "1080")
                mar.set(qn("w:right"), "1080")
                mar.set(qn("w:top"), "1656")
                mar.set(qn("w:bottom"), "576")
        pPr.append(sect)
        self.els.append(p._p)

    def install_before(self, end_el):
        for el in self.els:
            end_el.addprevious(el)


INLINE = re.compile(r"(\*\*.+?\*\*|`[^`]+`|\*[^*\n]+?\*)")


def add_runs(p, text):
    text = re.sub(r"\[([^\]]+)\]\([^)]+\)", r"\1", text)
    pos = 0
    for m in INLINE.finditer(text):
        if m.start() > pos:
            p.add_run(text[pos : m.start()])
        tok = m.group(0)
        if tok.startswith("**"):
            p.add_run(tok[2:-2]).bold = True
        elif tok.startswith("`"):
            r = p.add_run(tok[1:-1])
            r.font.name = "Consolas"
        else:
            p.add_run(tok[1:-1]).italic = True
        pos = m.end()
    if pos < len(text):
        p.add_run(text[pos:])


def png_size(path):
    with open(path, "rb") as f:
        d = f.read(24)
    if d[1:4] == b"PNG":
        return struct.unpack(">II", d[16:24])
    raise SystemExit("not a png: " + path)


def fit_width(path, maxw, maxh):
    w, h = png_size(path)
    width = maxw
    height = width * h / w
    if height > maxh:
        height = maxh
        width = height * w / h
    return Inches(width)


HEADING_STYLES = {1: "Heading 1", 2: "Heading 2", 3: "Heading 3", 4: "Heading 4", 5: "Heading 4"}

TABLE_SEP = re.compile(r"^\|[\s:\-|]+\|$")
IMG_ONLY = re.compile(r"^!\[\]\((.+?)\)$")
IMG = re.compile(r"!\[\]\((.+?)\)")


def resolve_img(md_path, rel):
    return os.path.normpath(os.path.join(os.path.dirname(md_path), rel))


def build_md(md_path, b, mode="plain", dsd_landscape=False):
    with open(md_path, encoding="utf-8-sig") as f:
        lines = f.read().splitlines()

    if mode == "plain":
        for idx, ln in enumerate(lines):
            if ln.startswith("## "):
                lines = lines[idx:]
                break

    i = 0
    while i < len(lines):
        s = lines[i].strip()
        if not s or s == "---":
            i += 1
            continue
        if s.startswith("```"):
            i += 1
            while i < len(lines) and not lines[i].strip().startswith("```"):
                i += 1
            i += 1
            continue

        if dsd_landscape and s.startswith("DSD (detallado):"):
            b.section_break(landscape=False)
            b.para("DSD (detallado):", tight=True)
            imgs = IMG.findall(s)
            if not imgs:
                i += 1
                while i < len(lines) and not IMG.findall(lines[i].strip()):
                    i += 1
                imgs = IMG.findall(lines[i].strip()) if i < len(lines) else []
            for rel in imgs:
                b.image(resolve_img(md_path, rel), LAND_W, LAND_H, tight=True)
            b.section_break(landscape=True)
            i += 1
            continue

        m = IMG_ONLY.fullmatch(s)
        if m:
            b.image(resolve_img(md_path, m.group(1)), PORTRAIT_W, PORTRAIT_H)
            i += 1
            continue

        if "![](" in s:
            before = s[: s.index("![](")].strip()
            if before.startswith(("- ", "* ")):
                before = before[2:].strip()
            if before:
                p = b.para()
                add_runs(p, before)
            for rel in IMG.findall(s):
                b.image(resolve_img(md_path, rel), PORTRAIT_W, PORTRAIT_H)
            i += 1
            continue

        m = re.match(r"^(#{1,6}) (.*)$", s)
        if m:
            level = len(m.group(1))
            txt = m.group(2).strip()
            if mode == "cu" and level >= 4:
                nums = txt.split(" ")[0].rstrip(".").split(".")
                if len(nums) == 2:
                    b.para(txt, "Heading 3")
                elif len(nums) >= 3:
                    b.para(txt)
                else:
                    b.para(txt, HEADING_STYLES[min(level, 5)])
            else:
                b.para(txt, HEADING_STYLES[min(level, 5)])
            i += 1
            continue

        if s.startswith("|"):
            rows = []
            while i < len(lines) and lines[i].strip().startswith("|"):
                raw = lines[i].strip()
                if not TABLE_SEP.fullmatch(raw):
                    cells = [c.strip() for c in raw.strip("|").split("|")]
                    rows.append(cells)
                i += 1
            if rows:
                b.table(rows)
            continue

        t = s
        if t.startswith("> "):
            t = t[2:]
        if t.startswith(("- ", "* ")):
            t = t[2:]
        p = b.para()
        add_runs(p, t.strip())
        i += 1


def md_for(prefix):
    pat = re.compile(r"^" + re.escape(prefix.rstrip()) + r"\s+.*\.md$")
    hits = [n for n in os.listdir(MD_DIR) if pat.match(n)]
    if not hits:
        raise SystemExit("no md for " + prefix)
    return os.path.join(MD_DIR, sorted(hits)[0])


def cu_files(section):
    pat = re.compile(r"^" + section + r"\.(\d+)\s+CU-.*\.md$")
    found = []
    for n in os.listdir(MD_DIR):
        m = pat.match(n)
        if m:
            found.append((int(m.group(1)), os.path.join(MD_DIR, n)))
    return [p for _, p in sorted(found)]


def replace_range(start_el, end_el, b):
    remove_range(start_el, end_el)
    b.install_before(end_el)


def main():
    global sample_tblpr
    normalize_sectprs()
    sample_tblpr = copy.deepcopy(doc.tables[0]._tbl.tblPr)

    for t in doc.tables:
        hdr = [c.text.strip() for c in t.rows[0].cells]
        if hdr[:2] == ["Fecha", "Versión"]:
            row = t.add_row()
            vals = [
                "18/11/2026",
                "v1.4.0",
                "Darío Zubaray",
                "Entrega recuperatoria: 21 casos de uso atómicos con un solo flujo básico, "
                "diagramas de secuencia con clases instanciadas, DER regenerado desde el script "
                "de creación y DSD de login en hoja apaisada agrandada.",
            ]
            for c, v in zip(row.cells, vals):
                c.text = ""
                add_runs(c.paragraphs[0], v)
            break

    s1_start = find_heading("Descripción global del Producto", "Heading 2")
    s1_end = find_heading("Especificación de Casos de Uso", "Heading 2")
    s4_end = find_heading("Otros Requisitos (No funcionales)", "Heading 2")
    s6_start = find_heading("6.1 CU-01", "Heading 3")
    s7_start = find_heading("Diagrama de Clases Global del Sistema", "Heading 2")
    s7_end = find_heading("DER Global del Sistema", "Heading 2")
    s8_end = body.find(qn("w:sectPr"))

    renumber_heading(s4_end, "5. Otros Requisitos (No funcionales)")
    renumber_heading(find_heading("Aspectos Técnicos", "Heading 2"), "6. Aspectos Técnicos")

    b1 = Builder()
    build_md(os.path.join(MD_DIR, "secciones-1-a-3-para-word.md"), b1, mode="plain")
    replace_range(s1_start, s1_end, b1)
    print("secciones 1-3 regeneradas")

    b4 = Builder()
    b4.para("4. Especificación de Casos de Uso", "Heading 2")
    build_md(md_for("4.1 "), b4, mode="cu")
    for pth in cu_files("4"):
        build_md(pth, b4, mode="cu")
    replace_range(s1_end, s4_end, b4)
    print("seccion 4: %d CUs regenerados" % len(cu_files("4")))

    b6 = Builder()
    for idx, pth in enumerate(cu_files("6")):
        build_md(pth, b6, mode="cu", dsd_landscape=(idx == 0))
    replace_range(s6_start, s7_start, b6)
    print("seccion 6: %d CUs regenerados (DSD login apaisado)" % len(cu_files("6")))

    b7 = Builder()
    b7.para("7. Diagrama de Clases Global del Sistema", "Heading 2")
    dc_dir = os.path.join(MER_DIR, "7. diagramas-clases")
    for n in sorted(os.listdir(dc_dir)):
        if n.endswith(".png"):
            b7.image(os.path.join(dc_dir, n), PORTRAIT_W, PORTRAIT_H)
    replace_range(s7_start, s7_end, b7)
    print("seccion 7: diagramas de clases insertados")

    b8 = Builder()
    b8.para("8. DER Global del Sistema", "Heading 2")
    b8.image(os.path.join(MER_DIR, "8. diagramas-entidad-relacion", "8.er.png"), PORTRAIT_W, PORTRAIT_H)
    replace_range(s7_end, s8_end, b8)
    print("seccion 8: DER regenerado")

    for child in body.iterchildren():
        if child.tag != qn("w:p"):
            continue
        p = Paragraph(child, doc)
        if p.text.strip():
            break
        if p._p.find(".//" + qn("w:drawing")) is not None:
            pPr = p._p.find(qn("w:pPr"))
            if pPr is not None:
                st = pPr.find(qn("w:pStyle"))
                if st is not None and st.get(qn("w:val")) == "Heading2":
                    st.set(qn("w:val"), "Normal")
                    print("portada: banner sin texto salio del TOC (era Heading2)")

    settings = doc.settings.element
    if settings.find(qn("w:updateFields")) is None:
        el = OxmlElement("w:updateFields")
        el.set(qn("w:val"), "true")
        settings.append(el)

    doc.save(DOCX_PATH)
    print("saved:", DOCX_PATH)


if __name__ == "__main__":
    main()
