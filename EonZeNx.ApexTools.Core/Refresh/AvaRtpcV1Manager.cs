using EonZeNx.ApexTools.Core.Processors;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public class AvaRtpcV1Manager : GenericAvaFileManager
    {
        public AvaRtpcV1Manager(string path = "") : base(EFourCc.Rtpc, 1, path) { }


        public override StepResult Process()
        {
            return new ();
        }

        protected override StepResult ImportBinary() { return new (); }
        protected override StepResult ExportConverted() { return new (); }
        protected override StepResult ImportConverted() { return new (); }
        protected override StepResult ExportBinary() { return new (); }
    }
}