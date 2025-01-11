namespace HaircutManager.Models
{
    public enum OtpType
    {
        Exp_negAX,  //exp(-a*x),
        Ln_AX,      //lg(a*x),
        ASin_X,     //a*sin(x),
        ALn_X,      //a*ln(x),
        AdivX,      //a/x,
        Ln_AdivX,   //lg(a/x),
        XdivSin_A,  //x/sin(a),
        ASin_1divX, //a*sin(1/x),
        Tg_AX,      //tg(a*x),
        ALn_2X,      //a*ln(2+x),
        Random12    //Random 12 characters
    }
}