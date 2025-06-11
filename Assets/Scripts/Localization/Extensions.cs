using TMPro;

namespace RemoteEducation.Localization
{
    public static class Extensions
    {
        public static string Localize(this string str)
        {
            return Localizer.Localize(str);
        }

        public static string LocalizePassthrough(this string str)
        {
            return Localizer.LocalizePassthrough(str);
        }

        public static void Localize(this TextMeshProUGUI textComponent)
        {
            textComponent.text = Localizer.Localize(textComponent.text);
        }

        public static void Localize(this TextMeshProUGUI textComponent, string token)
        {
            textComponent.text = Localizer.Localize(token);
        }

        public static void LocalizePassthrough(this TextMeshProUGUI textComponent)
        {
            textComponent.text = Localizer.LocalizePassthrough(textComponent.text);
        }

        public static void LocalizePassthrough(this TextMeshProUGUI textComponent, string token)
        {
            textComponent.text = Localizer.LocalizePassthrough(token);
        }
    }
}
