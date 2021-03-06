using System;

namespace AllinaHealth.WebAPI.Areas.HelpPage.SampleGeneration
{
    /// <summary>
    /// This represents a preformatted text sample on the help page. There's a display template named TextSample associated with this class.
    /// </summary>
    public class TextSample
    {
        public TextSample(string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public string Text { get; }

        public override bool Equals(object obj)
        {
            return obj is TextSample other && Text == other.Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}