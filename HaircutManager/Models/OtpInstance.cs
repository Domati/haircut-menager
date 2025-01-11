using System.Security.Claims;

namespace HaircutManager.Models
{
    public class OtpInstance
    {
        public int id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public double ParameterA { get; set; } = 0;
        public double ParameterB { get; set; } = 0;
        public OtpType OtpType { get; set; } = OtpType.Random12;
        public string Answer { get; set; } = "";
        public bool IsUsed { get; set; } = false;

        public OtpInstance(){}
        public OtpInstance(double parameterA, double parameterB, OtpType otpType, string answer)
        {
            ParameterA = parameterA;
            ParameterB = parameterB;
            OtpType = otpType;
            Answer = answer;
        }
    }
}
