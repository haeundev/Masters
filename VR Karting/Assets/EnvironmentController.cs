using System;
using System.Collections.Generic;
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
    
    [SerializeField] private List<GameObject> clearObjects;
    [SerializeField] private List<GameObject> rainObjects;
    [SerializeField] private List<GameObject> snowObjects;
    [SerializeField] private List<GameObject> morningObjects;
    [SerializeField] private List<GameObject> afternoonObjects;
    [SerializeField] private List<GameObject> eveningObjects;
    [SerializeField] private List<GameObject> nightObjects;

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
        
        GlobalInfo.WeatherType = weatherType;
        GlobalInfo.TimeOfDay = timeOfDay;
        
        clearObjects.ForEach(go => go.SetActive(false));
        rainObjects.ForEach(go => go.SetActive(false));
        snowObjects.ForEach(go => go.SetActive(false));
        morningObjects.ForEach(go => go.SetActive(false));
        afternoonObjects.ForEach(go => go.SetActive(false));
        eveningObjects.ForEach(go => go.SetActive(false));
        nightObjects.ForEach(go => go.SetActive(false));

        if (weatherType != WeatherType.Clear)
            foreach (var obj in weatherObjects)
                obj.Value.SetActive(obj.Key == weatherType);
        else
            foreach (var obj in weatherObjects)
                obj.Value.SetActive(false);

        RenderSettings.skybox = skyboxByTimeOfDay[timeOfDay];
        directionalLight.color = colorByTimeOfDay[timeOfDay];
        
        switch (timeOfDay)
        {
            case TimeOfDay.Morning:
                morningObjects.ForEach(go => go.SetActive(true));
                break;
            case TimeOfDay.Afternoon:
                afternoonObjects.ForEach(go => go.SetActive(true));
                break;
            case TimeOfDay.Evening:
                eveningObjects.ForEach(go => go.SetActive(true));
                break;
            case TimeOfDay.Night:
                nightObjects.ForEach(go => go.SetActive(true));
                break;
        }

        switch (weatherType)
        {
            case WeatherType.Clear:
                clearObjects.ForEach(go => go.SetActive(true));
                break;
            case WeatherType.Rainy:
                rainObjects.ForEach(go => go.SetActive(true));
                break;
            case WeatherType.Snowy:
                snowObjects.ForEach(go => go.SetActive(true));
                break;
        }
        
        Debug.Log($"Environment set to {weatherType} - {timeOfDay}");
    }
}