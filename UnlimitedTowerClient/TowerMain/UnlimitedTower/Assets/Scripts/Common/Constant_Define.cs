﻿using UnityEngine;

#region DEFINE

//공통 사항을 모아둔 클래스.
public class DEFINE
{
    public static readonly int MonsterMinFormationNum = 5;
    public static readonly int MonsterMaxFormationNum = 9;
    public static readonly int ServantMinFormationNum = 0;
    public static readonly int ServantMaxFormationNum = 4;

	// MIN - MAX Status
	public static readonly int SERVANT_MIN_STATUS = 30;
	public static readonly int SERVANT_MAX_STATUS = 60;

    // legendary
    public static readonly int MONSTER_MIN_STATUS = 75;
    public static readonly int MONSTER_MAX_STATUS = 150;
    // status = max_status - (monster add status * grade)
    public static readonly int MONSTER_GRADE_ADD_STATUS = 10;

    public static readonly int RAND_STATUS = 10;
    public static readonly int MAX_LEVEL = 50;
    public static readonly int MAX_EXP = 5000;
    public static readonly int BONUS_STAT = 1;
    public static readonly int MAX_MONSTER_UPGRADE_COUNT = 9;

    // SHADER Name
    public static readonly string OUTLINE_SHADER = "Outlined/UltimateOutline";
    public static readonly string BASIC_SHADER = "Mobile/Unlit (Supports Lightmap)";

    // Party Info
    public static readonly int PARTY_MAX_NUM = 10;

    // Battle Position
    public static readonly float START_X = -1.5f;
    public static readonly float START_Z = -5.0f;
    public static readonly float DISTANCE_OFFSET = 1.0f;

    // 캐릭터 끼리 최소 만나기 직전의 최소 거리
    public static readonly float STOP_DISTANCE = 0.5f;

    // 공격 끝나고 되돌아 올때 거리 보정하는 기준.
    public static readonly float MIN_DISTANCE = 0.3f;

    // 움직이는 속도
    public static readonly float MOVING_SPEED = 4.0f;
    public  static float ANIMATION_SPEED = 2.0f;

    // 테스트 상수
    public static Vector3 PLAYER_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, -5.0f);
    public static Vector3 ENEMY_BACKLINE_CENTER_POS = new Vector3(0.0f, 0.0f, 5.0f);

    public static readonly int HERO_FORMATION_NUM = 7;
    public static readonly int TEST_STATUS_VALUE = 1;



    public static Vector3 GetBattlePosition(int partyIndex, FORMATION_TYPE type, int num)
    {
        Vector3 position = new Vector3();
        int posOffset = 0;
        float y = 0.0f;
        if(type == FORMATION_TYPE.PLAYER)
        {
            posOffset = 1;
        }
        else
        {
            posOffset = -1;
        }

        switch (partyIndex)
        {
            case 0:
                position = new Vector3(START_X + DISTANCE_OFFSET * 1, y, START_Z * posOffset + 1.0f);
                break;
            case 1:
                position = new Vector3(START_X + DISTANCE_OFFSET * 2, y, START_Z * posOffset + 1.0f);
                break;
            case 2:
                position = new Vector3(START_X + DISTANCE_OFFSET * 3, y, START_Z * posOffset + 1.0f);
                break;
            case 3:
                position = new Vector3(START_X + DISTANCE_OFFSET * 1, y, START_Z * posOffset);
                break;
            case 4:
                position = new Vector3(START_X + DISTANCE_OFFSET * 2, y, START_Z * posOffset);
                break;
            case 5:
                position = new Vector3(START_X + DISTANCE_OFFSET * 3, y, START_Z * posOffset);
                break;
        }

        return position;
    }

    //임시코드 : 추후에 적절하게 수정 필요
    public static SIZE_TYPE GetCharacterSize(CHARACTER_NUM charType)
    {
        SIZE_TYPE charSize;

        if (charType == CHARACTER_NUM.Mst_Knight || charType == CHARACTER_NUM.Mst_Anubis || charType == CHARACTER_NUM.Mst_DarkKnight
            || charType == CHARACTER_NUM.Mst_WolfMan || charType == CHARACTER_NUM.Mst_Robot || charType == CHARACTER_NUM.Mst_Minotaurus
            || charType == CHARACTER_NUM.Mst_Robot_2)
        {
            charSize = SIZE_TYPE.MIDDLE;
        }
        else if (charType == CHARACTER_NUM.Mst_Death || charType == CHARACTER_NUM.Mst_Giant || charType == CHARACTER_NUM.Mst_BirdMan)
        {
            charSize = SIZE_TYPE.BIG;
        }
        else
        {
            charSize = SIZE_TYPE.SMALL;
        }
        return charSize;
    }

    //등급에 따른 컬러값
    public static Color GetGradeColor(GRADE_TYPE type) {
        Color grade_color;
        switch (type)
        {
            case GRADE_TYPE.COMMON:
                grade_color = new Color(1f, 1f, 1f);
                break;
            case GRADE_TYPE.UNCOMMON:
                grade_color = new Color(44 / 255f, 1f, 121 / 255f);
                break;
            case GRADE_TYPE.RARE:
                grade_color = new Color(82 / 255f, 192 / 255f, 1f);
                break;
            case GRADE_TYPE.UNIQUE:
                grade_color = new Color(231 / 255f, 75 / 255f, 1f);
                break;
            case GRADE_TYPE.LEGENDARY:
                grade_color = new Color(1f, 150 / 250f, 39 / 255f);
                break;
            default:
                grade_color = Color.black;
                break;
        }
        return grade_color;
    }

}

#endregion