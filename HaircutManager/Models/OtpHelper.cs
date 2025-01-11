using HaircutManager.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HaircutManager.Models
{
    public static class OtpHelper
    {
        public static OtpInstance GenerateOtpClaim(OtpType otpType)
        {
            Random random = new Random();
            double parameterA = random.Next(1, 10);
            double parameterB = random.Next(1, 10);
            string answer = "";
            switch (otpType)
            {
                case OtpType.Exp_negAX:
                    answer = Math.Exp(-parameterA * parameterB).ToString();
                    break;
                case OtpType.Ln_AX:
                    answer = Math.Log(parameterA * parameterB).ToString();
                    break;
                case OtpType.ASin_X:
                    answer = (parameterA * Math.Sin(parameterB)).ToString();
                    break;
                case OtpType.ALn_X:
                    answer = (parameterA * Math.Log(parameterB)).ToString();
                    break;
                case OtpType.AdivX:
                    answer = (parameterA / parameterB).ToString();
                    break;
                case OtpType.Ln_AdivX:
                    answer = Math.Log(parameterA / parameterB).ToString();
                    break;
                case OtpType.XdivSin_A:
                    answer = (parameterB / Math.Sin(parameterA)).ToString();
                    break;
                case OtpType.ASin_1divX:
                    answer = (parameterA * Math.Sin(1 / parameterB)).ToString();
                    break;
                case OtpType.Tg_AX:
                    answer = Math.Tan(parameterA * parameterB).ToString();
                    break;
                case OtpType.ALn_2X:
                    answer = (parameterA * Math.Log(2 + parameterB)).ToString();
                    break;
                case OtpType.Random12:
                    answer = Guid.NewGuid().ToString().Substring(0, 12);
                    break;
            }
            return new OtpInstance(parameterA, parameterB, otpType, answer);
        }
        public static OtpInstance GenerateRandomOtpClaim()
        {
            Random random = new Random();
            OtpType otpType = (OtpType)random.Next(0, 11);
            return GenerateOtpClaim(otpType);
        }

        public static bool CheckOtpClaim(OtpInstance? otpClaim, string answer)
        {
            if (otpClaim == null)
            {
                return true;
            }

            return otpClaim.Answer == answer;
        }

        public static bool AssignOtpClaimToUser(UserManager<ApplicationUser> userManager, ApplicationUser user, OtpType otpType)
        {
            OtpInstance otpClaim = GenerateOtpClaim(otpType);

            return userManager.AddClaimAsync(user, new Claim("OtpClaim", OtpHelper.GetTypeName(otpType))).Result.Succeeded;
        }

        public static dynamic GetOtpTypes()
        {
            return Enum.GetValues(typeof(OtpType));
        }

        internal static string GetTypeName(OtpType otpType)
        {
            switch (otpType)
            {
                case OtpType.Exp_negAX:
                    return "Exp(-AX)";
                case OtpType.Ln_AX:
                    return "Ln(AX)";
                case OtpType.ASin_X:
                    return "ASin(X)";
                case OtpType.ALn_X:
                    return "ALn(X)";
                case OtpType.AdivX:
                    return "A/X";
                case OtpType.Ln_AdivX:
                    return "Ln(A/X)";
                case OtpType.XdivSin_A:
                    return "X/Sin(A)";
                case OtpType.ASin_1divX:
                    return "ASin(1/X)";
                case OtpType.Tg_AX:
                    return "Tg(AX)";
                case OtpType.ALn_2X:
                    return "ALn(2+X)";
                case OtpType.Random12:
                    return "Random 12";
                default:
                    return "Random 12";
            }
        }
    }
}
