using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private bool playerIsRighthanded = true;
    [SerializeField]
    private TextMeshPro[] clocks;

    private int hours;
    private int min;
    private int seconds;
    private bool paused;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        StopTimer();
    }
    public void StartTimer()
    {
        StartCoroutine("IncreaseTimer");
    }

    public void StopTimer()
    {
        StopCoroutine("IncreaseTimer");
    }
    public void PauseTimer(bool isPaused)
    {
        paused = isPaused;
    }

    public void ResetTimer()
    {
        seconds = 0;
        min = 0;
        hours = 0;
    }

    private IEnumerator IncreaseTimer()
    {
        if(!paused)
        {
            foreach(TextMeshPro textMesh in clocks)
            {
                textMesh.text = DisplayTime();
                AddTime(1);
            }
        }
        yield return new WaitForSeconds(1);
    }

    private void AddTime(int seconds, int minutes=0, int hours = 0)
    {
        if(seconds + this.seconds >= 60)
        {
            minutes++;
            this.seconds = this.seconds + seconds - 60;
        } else
        {
            this.seconds += seconds;
        }

        if(minutes + this.min >= 60)
        {
            hours++;
            this.min = this.min + minutes - 60;
        } else
        {
            this.min += minutes;
        }

        if(hours + this.hours > 99)
        {
            this.hours = 99;
            this.min = 59;
            this.seconds = 59;
        } else
        {
            this.hours += hours;
        }
        
    }

    private string DisplayTime()
    {
        return string.Format("{0}:{1}",min,seconds);
    }
}
