using UnityEngine;
using System.Threading.Tasks;

public static class TimeHelper
{
    public static async Task WaitForSeconds(float waitTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    } 
}
