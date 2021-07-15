using System.Diagnostics;
using System.IO;

namespace EonZeNx.ApexTools.Core.Refresh
{
    public enum EStepInstruction
    {
        Copy,
        Move,
        Process,
        Launch
    }
    
    public enum EStepResult
    {
        Success,
        Info,
        Warning,
        Error
    }

    public interface ISubStep
    {
        /// <summary>
        /// Recursive substep function.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public StepResult SubStep(string[] paths);
    }

    
    /// <summary>
    /// Represents the result of a step execution.
    /// </summary>
    public struct StepResult
    {
        public EStepResult Result { get; set; }
        public string Message { get; set; }

        
        public StepResult(EStepResult result = EStepResult.Success, string message = "OK")
        {
            Result = result;
            Message = message;
        }
    }
    
    
    /// <summary>
    /// Represents a single instruction for the tools to execute.
    /// </summary>
    public abstract class Step
    {
        public EStepInstruction Instruction;
        public StepResult Result;
        public string Target;
        public string Additional;

        public Step(string target, string additional)
        {
            Target = target;
            Additional = additional;
        }

        /// <summary>
        /// Executes the instruction.
        /// </summary>
        /// <returns>The result of the instruction.</returns>
        public abstract StepResult Execute();

        /// <summary>
        /// Sanity check before running the step.
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }

    public class CopyStep : Step, ISubStep
    {
        public bool DstIsDirectory;
        
        public CopyStep(string target, string additional) : base(target, additional)
        {
            Instruction = EStepInstruction.Copy;
        }
        
        
        public override bool IsValid()
        {
            var targetIsValid = File.Exists(Target) || Directory.Exists(Target);

            if (targetIsValid) return true;
            
            Result = new StepResult(EStepResult.Error, $"Path not found '{Target}'");
            return false;
        }
        
        public override StepResult Execute()
        {
            Result = new StepResult(EStepResult.Success, "OK");
            
            if (!IsValid()) return Result;

            DstIsDirectory = !Path.HasExtension(Additional) && !File.Exists(Additional);
            if (DstIsDirectory)
            {
                // Is a directory
                Directory.CreateDirectory(Additional);
            }
            
            if (File.Exists(Target))
            {
                // Target is file
                var fullDstPath = Additional;
                if (DstIsDirectory)
                {
                    fullDstPath = Path.Combine(Additional, Path.GetFileName(Target) ?? Target);
                }
                
                File.Copy(Target, fullDstPath, true);
            }
            else
            {
                // Target is directory
                var files = Directory.GetFiles(Target);
                var directories = Directory.GetDirectories(Target);

                var filesResult = SubStep(files);
                var directoriesResult = SubStep(directories);

                if (filesResult.Result == EStepResult.Error) Result = filesResult;
                if (directoriesResult.Result == EStepResult.Error) Result = directoriesResult;
            }

            return Result;
        }

        
        public StepResult SubStep(string[] paths)
        {
            // Copy directories.
            foreach (var p in paths)
            {
                var fileName = Path.GetFileName(p);
                var subStep = new CopyStep(p, Path.Combine(Additional, fileName));
                var result = subStep.Execute();

                if (result.Result == EStepResult.Error) return result;
            }

            return new StepResult();
        }
    }
    
    public class MoveStep : Step, ISubStep
    {
        public bool DstIsDirectory;
        
        public MoveStep(string target, string additional) : base(target, additional)
        {
            Instruction = EStepInstruction.Move;
        }

        
        public override bool IsValid()
        {
            var targetIsValid = File.Exists(Target) || Directory.Exists(Target);

            if (targetIsValid) return true;
            
            Result = new StepResult(EStepResult.Error, $"Path not found '{Target}'");
            return false;
        }
        
        public override StepResult Execute()
        {
            Result = new StepResult(EStepResult.Success, "OK");
            
            if (!IsValid()) return Result;
            
            DstIsDirectory = !Path.HasExtension(Additional) && !File.Exists(Additional);
            if (DstIsDirectory)
            {
                // Is a directory
                Directory.CreateDirectory(Additional);
            }
            
            if (File.Exists(Target))
            {
                // Target is file
                var fullDstPath = Additional;
                if (DstIsDirectory)
                {
                    fullDstPath = Path.Combine(Additional, Path.GetFileName(Target) ?? Target);
                }
                
                File.Move(Target, fullDstPath, true);
            }
            else
            {
                // Target is directory
                var files = Directory.GetFiles(Target);
                var directories = Directory.GetDirectories(Target);

                var filesResult = SubStep(files);
                var directoriesResult = SubStep(directories);

                if (filesResult.Result == EStepResult.Error) Result = filesResult;
                if (directoriesResult.Result == EStepResult.Error) Result = directoriesResult;
            }

            return Result;
        }

        public StepResult SubStep(string[] paths)
        {
            // Move directories.
            foreach (var p in paths)
            {
                var fileName = Path.GetFileName(p);
                var subStep = new MoveStep(p, Path.Combine(Additional, fileName));
                var result = subStep.Execute();

                if (result.Result == EStepResult.Error) return result;
            }

            return new StepResult();
        }
    }

    public class LaunchStep : Step
    {
        public LaunchStep(string target, string additional) : base(target, additional)
        {
            Instruction = EStepInstruction.Launch;
        }


        public override bool IsValid()
        {
            var targetIsValid = File.Exists(Target);

            if (targetIsValid) return false;
            
            Result = new StepResult(EStepResult.Error, $"Path not found. '{Target}'");
            return false;

        }

        public override StepResult Execute()
        {
            Result = new StepResult();

            if (!IsValid()) return Result;

            Process.Start(Target, Additional);

            return Result;
        }
    }
}