using UnityEngine;

namespace SPD.EditorAttributes
{
    public class CustomToggleAttribute : PropertyAttribute
    {
        public readonly string title;


        public CustomToggleAttribute()
        {
            this.title = "";
        }

        public CustomToggleAttribute(string _title)
        {
            this.title = _title;
        }
    }
}