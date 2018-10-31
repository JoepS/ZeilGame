using LitJson;
using System;
using UnityEngine;

public class WeatherData
{
    CoordinateOld coord;
    WeatherOld weather;
    MainOld main;
    //double visibility;
    WindOld wind;
    CloudsOld clouds;
    int dt;
    SysOld sys;
    string cityName;


    public WeatherData(JsonData data)
    {
        this.coord = JsonMapper.ToObject<CoordinateOld>(data["coord"].ToJson());
        this.weather = JsonMapper.ToObject<WeatherOld>(data["weather"].ToJson().Replace("[", "").Replace("]", ""));
        this.main = JsonMapper.ToObject<MainOld>(data["main"].ToJson());
        this.wind = JsonMapper.ToObject<WindOld>(data["wind"].ToJson());
        this.clouds = JsonMapper.ToObject<CloudsOld>(data["clouds"].ToJson());
        this.dt = (int)data["dt"];
        this.sys = JsonMapper.ToObject<SysOld>(data["sys"].ToJson());
        this.cityName = data["name"].ToString();
    }

    public double getWindDir()
    {
        return wind.deg;
    }

    public double getWindSpeed()
    {
        return wind.speed;
    }

    public string getName()
    {
        return (string)this.cityName;
    }

    public double getTemperature()
    {
        return main.temperature;
    }

    //public double getVisibility()
    //{
    //    return visibility;
    //}

    public int getClouds()
    {
        return clouds.all;
    }

    public SysOld getSys()
    {
        return sys;
    }

    public DateTime getUpdateTime()
    {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(dt).ToLocalTime();
        return dtDateTime;
    }

    public override string ToString()
    {
        string s = "";
        s += "Coordinate: " + coord.lat + "/" + coord.lon + "\n";
        s += "Weather: " + weather.id + ", " + weather.main + ", " + weather.main + ", " + weather.icon + "\n";
        s += "Main: " + main.temperature + ", " + main.pressure + ", " + main.humidity + ", " + main.temprature_min + ", " + main.temprature_max + "\n";
        s += "wind: " + wind.speed + ", " + wind.deg + "\n";
        return s;
    }

    public string getWeatherImage()
    {
        return weather.icon;
    }

    public string getWeatherText()
    {
        return this.weather.description;
    }
}

public class CoordinateOld
{
    public double lat;
    public double lon;
}

public class WeatherOld
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

public class MainOld
{
    public double temp;
    public double temperature
    {
        get
        {
            return temp - 273.15;
        }
    }
    public double pressure;
    public double humidity;
    public double temp_min;
    public double temprature_min
    {
        get
        {
            return temp_min - 273.15;
        }
    }
    public double temp_max;
    public double temprature_max
    {
        get
        {
            return temp_max - 273.15;
        }
    }
}

public class WindOld
{
    public double speed;
    public double deg;
}

public class CloudsOld
{
    public int all;
}

public class SysOld
{
    public int type;
    public int id;
    public double message;
    public string country;
    public double sunrise;
    public double sunset;
}
