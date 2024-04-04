using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class EnvironmentController : MonoBehaviour
{
    [Serializable]
    public class WeatherObjects : SerializedDictionary<WeatherType, GameObject>
    {
    }

    [Serializable]
    public class SkyboxByTimeOfDay : SerializedDictionary<TimeOfDay, Material>
    {
    }

    [Serializable]
    public class ColorByTimeOfDay : SerializedDictionary<TimeOfDay, Color>
    {
    }

    public enum WeatherType
    {
        Clear,
        Rainy,
        Snowy
    }

    public enum TimeOfDay
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    public WeatherType weatherType;
    public TimeOfDay timeOfDay;

    public WeatherObjects weatherObjects;
    public SkyboxByTimeOfDay skyboxByTimeOfDay;
    public ColorByTimeOfDay colorByTimeOfDay;

    [SerializeField] private Light directionalLight;

    [Button]
    public void SetRandomEnvironment()
    {
        var randomWeather = (WeatherType)Random.Range(0, Enum.GetValues(typeof(WeatherType)).Length);
        var randomTime = (TimeOfDay)Random.Range(0, Enum.GetValues(typeof(TimeOfDay)).Length);

        SetEnvironment(randomWeather, randomTime);
    }

    public void SetEnvironment(WeatherType weather, TimeOfDay time)
    {
        weatherType = weather;
        timeOfDay = time;

        if (weatherType != WeatherType.Clear)
            foreach (var obj in weatherObjects)
                obj.Value.SetActive(obj.Key == weatherType);
        else
            foreach (var obj in weatherObjects)
                obj.Value.SetActive(false);

        RenderSettings.skybox = skyboxByTimeOfDay[timeOfDay];
        directionalLight.color = colorByTimeOfDay[timeOfDay];
    }
}