using EonZeNx.ApexTools.Core.Processors;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public class AvaAafV1Manager : GenericAvaFileManager
    {
        public AvaAafV1Manager(string path = "") : base(EFourCc.Aaf, 1, path) { }


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