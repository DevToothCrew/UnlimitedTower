using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Json 파일 형태
// Sample

public enum AnimalType
{
    DOG, CAT, RABBIT
}

// 보내는 데이터 형식을 아래와 같이 정의해줄것.
public class JSONStructure
{
    public AnimalType type;
    public string name;
    public double age;
    public bool cute;
}