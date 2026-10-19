using System;

namespace BLL.Helpers
{
    public static class IntegrityStateBLL
    {
        #region Propiedades
        public static bool LastScanCompleted { get; private set; }
        public static bool LastScanHasFailures { get; private set; }
        public static DateTime? LastScanAt { get; private set; }
        #endregion

        #region Métodos
        public static void RecordScan(int violationCount)
        {
            LastScanCompleted = true;
            LastScanHasFailures = violationCount > 0;
            LastScanAt = DateTime.Now;
        }

        public static void Reset()
        {
            LastScanCompleted = false;
            LastScanHasFailures = false;
            LastScanAt = null;
        }
        #endregion
    }
}
