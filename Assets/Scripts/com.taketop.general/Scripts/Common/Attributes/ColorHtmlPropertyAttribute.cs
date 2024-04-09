using UnityEngine;


namespace Module.General
{
	public enum TypeHighlighting
	{
		All,
		OnlyField
	}
	
	
	public class ColorHtmlPropertyAttribute : PropertyAttribute
	{
		public Color Color;
		public TypeHighlighting TypeHighlighting;

		// Возможность передавать в атрибут сразу цвет удобно, но UnityEngine.Color передать в атрибут нельзя. Можно передавать только простые типы.
		// Думал что структура System.Drawing.KnownColor спасет, но эта структура не хочет работать в Unity. Возможно кто-то найдет решение.
		// public ColorHtmlPropertyAttribute(KnownColor _color) 
		// {
		// 	System.Drawing.Color knownColor = System.Drawing.Color.FromKnownColor(_color);
		// 	this.color = new UnityEngine.Color(knownColor.R, knownColor.G, knownColor.B, knownColor.A);
		// }


		/// <summary>
		/// Строки, начинающиеся с '#', будут анализироваться как шестнадцатеричные. Если альфа не указана, по умолчанию используется FF.
		/// Строки, которые не начинаются с символа '#', будут анализироваться как буквальные цвета,
		/// при этом поддерживаются следующие цвета : красный, голубой, синий, темно-синий, светло-голубой, фиолетовый, желтый, салатовый,
		/// фуксия, белый, серебристый, серый, черный, оранжевый, коричневый. , бордовый, зеленый, оливковый, темно-синий, бирюзовый, голубой, пурпурный ..
		/// </summary>
		/// <param name="color"></param>
		/// <param name="typeHighlighting">Тип подсветки: All - Окрасить все, OnlyField - окрасить только область поля</param>
		public ColorHtmlPropertyAttribute(string color, TypeHighlighting typeHighlighting)
		{
			Color MyColour = Color.clear;
			ColorUtility.TryParseHtmlString(color, out MyColour);
			Color = MyColour;
			TypeHighlighting = typeHighlighting;
		}


		public ColorHtmlPropertyAttribute(float r, float g, float b, float a, TypeHighlighting typeHighlighting)
		{
			Color = new Color(r, g, b, a);
			TypeHighlighting = typeHighlighting;
		}
	}
}
