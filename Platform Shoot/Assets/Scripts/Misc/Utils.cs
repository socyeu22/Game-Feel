using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void RunAfterDelay(MonoBehaviour monoBehaviour, float delay, Action task) { // Hàm này sẽ nhận vào tham số kiểu Action(là một method có kiểu trả về là void)
    monoBehaviour.StartCoroutine(RunAfterDelayRoutine(delay, task));
  }


    // Hàm này sẽ chạy một method sau một khoảng thời gian delay
    private static IEnumerator RunAfterDelayRoutine(float delay, Action task) {
    yield return new WaitForSeconds(delay); // Chờ trong thời gian delay
    task.Invoke(); // Sau đó chạy method task được truyền vào
  }
}
