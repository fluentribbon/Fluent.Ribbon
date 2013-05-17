namespace FluentTest
{
    public class DatePicker 
#if NET35
       : Microsoft.Windows.Controls.DatePicker
#else
       : System.Windows.Controls.DatePicker
#endif
    {
    }
}