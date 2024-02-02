using UnityEngine;
using System.Collections;
using TMPro;

[System.Serializable]
public class ArrayLayout
{

	[System.Serializable]
	public struct rowData
	{
		public TextMeshProUGUI[] row;
	}

	public rowData[] rows = new rowData[4]; //Grid of 4x4
}
