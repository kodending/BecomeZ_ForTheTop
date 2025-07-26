using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleGameData
{
    //구굴 시트 게임 데이터 저장관리하는 곳

    //구글 시트게임 데이터 리스트화

    public List<Dictionary<string, object>> recordDataList = new List<Dictionary<string, object>>();

    //게임 데이터 카테고리 타이틀 저장
    public List<Dictionary<int, object>> recordTitleList = new List<Dictionary<int, object>>();
}
