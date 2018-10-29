using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RectangularWeatherData
{
    public int cod;
    public double calctime;
    public int cnt;
    public List<WeatherData> list;
}
