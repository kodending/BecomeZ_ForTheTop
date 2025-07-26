#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Globalization;
using Unity.VisualScripting;
using PlayFab.DataModels;

public class SavePrefab : MonoBehaviour
{
    public GameObject saveObject;
    public Text savePathText;
    public void Save()
    {
#if UNITY_EDITOR
        if (savePathText == null || saveObject == null || string.IsNullOrEmpty(savePathText.text))
            return;
        // The paths to the mesh/prefab assets.
        string prefabPath = "Assets/" + savePathText.text + ".prefab";

        // Delete the assets if they already exist.
        AssetDatabase.DeleteAsset(prefabPath);

        // Save the transform's GameObject as a prefab asset.
        PrefabUtility.SaveAsPrefabAsset(saveObject, prefabPath);
#endif
    }

#if UNITY_EDITOR

    public GameObject charObjLobby;
    public GameObject charObjInGame;
    public PartSelectionController partSelectionController;

    public Text saveIndexName;

    public void SaveChar()
    {
        //savename
        if (!CheckNumber()) return;

        string inGamePrefabPath = "Assets/Resources/InGamePlayer/1.prefab";
        string lobbyPrefabPath = "Assets/Resources/LobbyPlayer/1.prefab";
        var inGamePlayerObj = PrefabUtility.LoadPrefabContents(inGamePrefabPath);
        var lobbyPlayerObj = PrefabUtility.LoadPrefabContents(lobbyPrefabPath);

        GameObject rootObj = inGamePlayerObj.transform.Find("Root_M").gameObject;
        GameObject ragRootObj = inGamePlayerObj.transform.Find("Root_M_Rag").gameObject;
        GameObject lobbyRootObj = lobbyPlayerObj.transform.Find("Root_M").gameObject;

        Transform[] allChildren = rootObj.GetComponentsInChildren<Transform>(includeInactive: true);
        Transform[] lobbyAllChildren = lobbyRootObj.GetComponentsInChildren<Transform>(includeInactive: true);
        Transform[] allChildrenRag = ragRootObj.GetComponentsInChildren<Transform>(includeInactive: true);

        GameObject savePart = new GameObject();

        #region 일반 root
        foreach (var child in allChildren)
        {
            //머리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headObject != null)
                {
                    savePart = partSelectionController.characterParts.headObject;
                    SaveOjbect(child, savePart);
                }

            }

            //헤어
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.hairObject != null)
                {
                    savePart = partSelectionController.characterParts.hairObject;
                    SaveOjbect(child, savePart);
                }
            }

            //머리 악세서리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headAccesoriesObject != null)
                {
                    savePart = partSelectionController.characterParts.headAccesoriesObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 숄더
            if (child.gameObject.name == "ShoulderR_Parent")
            {
                if (partSelectionController.characterParts.rightShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.rightShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 엘보
            if (child.gameObject.name == "ElbowR_Parent")
            {
                if (partSelectionController.characterParts.rightElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.rightElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무기
            if (child.gameObject.name == "WeaponR_Parent")
            {
                if (partSelectionController.characterParts.rightWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.rightWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 어꺠
            if (child.gameObject.name == "ShoulderL_Parent")
            {
                if (partSelectionController.characterParts.leftShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 엘보
            if (child.gameObject.name == "ElbowL_Parent")
            {
                if (partSelectionController.characterParts.leftElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.leftElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무기
            if (child.gameObject.name == "WeaponL_Parent")
            {
                if (partSelectionController.characterParts.leftWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.leftWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 쉴드
            if (child.gameObject.name == "Shield_Parent")
            {
                if (partSelectionController.characterParts.leftShieldObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShieldObject;
                    SaveOjbect(child, savePart);
                }
            }

            //가슴
            if (child.gameObject.name == "Chest_Parent")
            {
                if (partSelectionController.characterParts.chestObject != null)
                {
                    savePart = partSelectionController.characterParts.chestObject;
                    SaveOjbect(child, savePart);
                }
            }

            //스파인
            if (child.gameObject.name == "Spine_Parent")
            {
                if (partSelectionController.characterParts.spineObject != null)
                {
                    savePart = partSelectionController.characterParts.spineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //root
            if (child.gameObject.name == "Root_Parent")
            {
                if (partSelectionController.characterParts.lowerSpineObject != null)
                {
                    savePart = partSelectionController.characterParts.lowerSpineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 힙
            if (child.gameObject.name == "HipR_Parent")
            {
                if (partSelectionController.characterParts.rightHipObject != null)
                {
                    savePart = partSelectionController.characterParts.rightHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무릎
            if (child.gameObject.name == "KneeR_parent")
            {
                if (partSelectionController.characterParts.rightKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.rightKneeObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 힙
            if (child.gameObject.name == "HipL_Parent")
            {
                if (partSelectionController.characterParts.leftHipObject != null)
                {
                    savePart = partSelectionController.characterParts.leftHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무릎
            if (child.gameObject.name == "KneeL_parent")
            {
                if (partSelectionController.characterParts.leftKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.leftKneeObject;
                    SaveOjbect(child, savePart);
                }
            }
        }
        #endregion

        #region Ragdoll root
        foreach (var child in allChildrenRag)
        {
            //머리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headObject != null)
                {
                    savePart = partSelectionController.characterParts.headObject;
                    SaveOjbect(child, savePart);
                }

            }

            //헤어
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.hairObject != null)
                {
                    savePart = partSelectionController.characterParts.hairObject;
                    SaveOjbect(child, savePart);
                }
            }

            //머리 악세서리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headAccesoriesObject != null)
                {
                    savePart = partSelectionController.characterParts.headAccesoriesObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 숄더
            if (child.gameObject.name == "ShoulderR_Parent")
            {
                if (partSelectionController.characterParts.rightShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.rightShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 엘보
            if (child.gameObject.name == "ElbowR_Parent")
            {
                if (partSelectionController.characterParts.rightElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.rightElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무기
            if (child.gameObject.name == "WeaponR_Parent")
            {
                if (partSelectionController.characterParts.rightWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.rightWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 어꺠
            if (child.gameObject.name == "ShoulderL_Parent")
            {
                if (partSelectionController.characterParts.leftShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 엘보
            if (child.gameObject.name == "ElbowL_Parent")
            {
                if (partSelectionController.characterParts.leftElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.leftElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무기
            if (child.gameObject.name == "WeaponL_Parent")
            {
                if (partSelectionController.characterParts.leftWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.leftWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 쉴드
            if (child.gameObject.name == "Shield_Parent")
            {
                if (partSelectionController.characterParts.leftShieldObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShieldObject;
                    SaveOjbect(child, savePart);
                }
            }

            //가슴
            if (child.gameObject.name == "Chest_Parent")
            {
                if (partSelectionController.characterParts.chestObject != null)
                {
                    savePart = partSelectionController.characterParts.chestObject;
                    SaveOjbect(child, savePart);
                }
            }

            //스파인
            if (child.gameObject.name == "Spine_Parent")
            {
                if (partSelectionController.characterParts.spineObject != null)
                {
                    savePart = partSelectionController.characterParts.spineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //root
            if (child.gameObject.name == "Root_Parent")
            {
                if (partSelectionController.characterParts.lowerSpineObject != null)
                {
                    savePart = partSelectionController.characterParts.lowerSpineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 힙
            if (child.gameObject.name == "HipR_Parent")
            {
                if (partSelectionController.characterParts.rightHipObject != null)
                {
                    savePart = partSelectionController.characterParts.rightHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무릎
            if (child.gameObject.name == "KneeR_parent")
            {
                if (partSelectionController.characterParts.rightKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.rightKneeObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 힙
            if (child.gameObject.name == "HipL_Parent")
            {
                if (partSelectionController.characterParts.leftHipObject != null)
                {
                    savePart = partSelectionController.characterParts.leftHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무릎
            if (child.gameObject.name == "KneeL_parent")
            {
                if (partSelectionController.characterParts.leftKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.leftKneeObject;
                    SaveOjbect(child, savePart);
                }
            }
        }
        #endregion

        #region Lobby root
        foreach (var child in lobbyAllChildren)
        {
            //머리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headObject != null)
                {
                    savePart = partSelectionController.characterParts.headObject;
                    SaveOjbect(child, savePart);
                }

            }

            //헤어
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.hairObject != null)
                {
                    savePart = partSelectionController.characterParts.hairObject;
                    SaveOjbect(child, savePart);
                }
            }

            //머리 악세서리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headAccesoriesObject != null)
                {
                    savePart = partSelectionController.characterParts.headAccesoriesObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 숄더
            if (child.gameObject.name == "ShoulderR_Parent")
            {
                if (partSelectionController.characterParts.rightShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.rightShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 엘보
            if (child.gameObject.name == "ElbowR_Parent")
            {
                if (partSelectionController.characterParts.rightElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.rightElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무기
            if (child.gameObject.name == "WeaponR_Parent")
            {
                if (partSelectionController.characterParts.rightWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.rightWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 어꺠
            if (child.gameObject.name == "ShoulderL_Parent")
            {
                if (partSelectionController.characterParts.leftShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 엘보
            if (child.gameObject.name == "ElbowL_Parent")
            {
                if (partSelectionController.characterParts.leftElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.leftElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무기
            if (child.gameObject.name == "WeaponL_Parent")
            {
                if (partSelectionController.characterParts.leftWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.leftWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 쉴드
            if (child.gameObject.name == "Shield_Parent")
            {
                if (partSelectionController.characterParts.leftShieldObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShieldObject;
                    SaveOjbect(child, savePart);
                }
            }

            //가슴
            if (child.gameObject.name == "Chest_Parent")
            {
                if (partSelectionController.characterParts.chestObject != null)
                {
                    savePart = partSelectionController.characterParts.chestObject;
                    SaveOjbect(child, savePart);
                }
            }

            //스파인
            if (child.gameObject.name == "Spine_Parent")
            {
                if (partSelectionController.characterParts.spineObject != null)
                {
                    savePart = partSelectionController.characterParts.spineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //root
            if (child.gameObject.name == "Root_Parent")
            {
                if (partSelectionController.characterParts.lowerSpineObject != null)
                {
                    savePart = partSelectionController.characterParts.lowerSpineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 힙
            if (child.gameObject.name == "HipR_Parent")
            {
                if (partSelectionController.characterParts.rightHipObject != null)
                {
                    savePart = partSelectionController.characterParts.rightHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무릎
            if (child.gameObject.name == "KneeR_parent")
            {
                if (partSelectionController.characterParts.rightKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.rightKneeObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 힙
            if (child.gameObject.name == "HipL_Parent")
            {
                if (partSelectionController.characterParts.leftHipObject != null)
                {
                    savePart = partSelectionController.characterParts.leftHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무릎
            if (child.gameObject.name == "KneeL_parent")
            {
                if (partSelectionController.characterParts.leftKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.leftKneeObject;
                    SaveOjbect(child, savePart);
                }
            }
        }
        #endregion

        PrefabUtility.SaveAsPrefabAsset(inGamePlayerObj, inGamePrefabPath);
        PrefabUtility.UnloadPrefabContents(inGamePlayerObj);

        PrefabUtility.SaveAsPrefabAsset(lobbyPlayerObj, lobbyPrefabPath);
        PrefabUtility.UnloadPrefabContents(lobbyPlayerObj);
    }

    public void SaveEnemy()
    {
        //savename
        if (!CheckNumber()) return;

        string prefabPath = "Assets/Resources/Enemy/Enemy.prefab";
        //string prefabPath = "Assets/Resources/Enemy/EnemyTest.prefab";
        var enemyObj = PrefabUtility.LoadPrefabContents(prefabPath);

        GameObject rootObj = enemyObj.transform.Find("Root_M").gameObject;
        GameObject ragRootObj = enemyObj.transform.Find("Root_M_Rag").gameObject;

        Transform[] allChildren = rootObj.GetComponentsInChildren<Transform>(includeInactive: true);
        Transform[] allChildrenRag = ragRootObj.GetComponentsInChildren<Transform>(includeInactive: true);

        GameObject savePart = new GameObject();

        #region 일반 root
        foreach (var child in allChildren)
        {
            //머리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headObject != null)
                {
                    savePart = partSelectionController.characterParts.headObject;
                    SaveOjbect(child, savePart);
                }

            }

            //헤어
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.hairObject != null)
                {
                    savePart = partSelectionController.characterParts.hairObject;
                    SaveOjbect(child, savePart);
                }
            }

            //머리 악세서리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headAccesoriesObject != null)
                {
                    savePart = partSelectionController.characterParts.headAccesoriesObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 숄더
            if (child.gameObject.name == "ShoulderR_Parent")
            {
                if (partSelectionController.characterParts.rightShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.rightShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 엘보
            if (child.gameObject.name == "ElbowR_Parent")
            {
                if (partSelectionController.characterParts.rightElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.rightElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무기
            if (child.gameObject.name == "WeaponR_Parent")
            {
                if (partSelectionController.characterParts.rightWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.rightWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 어꺠
            if (child.gameObject.name == "ShoulderL_Parent")
            {
                if (partSelectionController.characterParts.leftShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 엘보
            if (child.gameObject.name == "ElbowL_Parent")
            {
                if (partSelectionController.characterParts.leftElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.leftElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무기
            if (child.gameObject.name == "WeaponL_Parent")
            {
                if (partSelectionController.characterParts.leftWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.leftWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 쉴드
            if (child.gameObject.name == "Shield_Parent")
            {
                if (partSelectionController.characterParts.leftShieldObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShieldObject;
                    SaveOjbect(child, savePart);
                }
            }

            //가슴
            if (child.gameObject.name == "Chest_Parent")
            {
                if (partSelectionController.characterParts.chestObject != null)
                {
                    savePart = partSelectionController.characterParts.chestObject;
                    SaveOjbect(child, savePart);
                }
            }

            //스파인
            if (child.gameObject.name == "Spine_Parent")
            {
                if (partSelectionController.characterParts.spineObject != null)
                {
                    savePart = partSelectionController.characterParts.spineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //root
            if (child.gameObject.name == "Root_Parent")
            {
                if (partSelectionController.characterParts.lowerSpineObject != null)
                {
                    savePart = partSelectionController.characterParts.lowerSpineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 힙
            if (child.gameObject.name == "HipR_Parent")
            {
                if (partSelectionController.characterParts.rightHipObject != null)
                {
                    savePart = partSelectionController.characterParts.rightHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무릎
            if (child.gameObject.name == "KneeR_parent")
            {
                if (partSelectionController.characterParts.rightKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.rightKneeObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 힙
            if (child.gameObject.name == "HipL_Parent")
            {
                if (partSelectionController.characterParts.leftHipObject != null)
                {
                    savePart = partSelectionController.characterParts.leftHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무릎
            if (child.gameObject.name == "KneeL_parent")
            {
                if (partSelectionController.characterParts.leftKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.leftKneeObject;
                    SaveOjbect(child, savePart);
                }
            }
        }
        #endregion

        #region Ragdoll root
        foreach (var child in allChildrenRag)
        {
            //머리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headObject != null)
                {
                    savePart = partSelectionController.characterParts.headObject;
                    SaveOjbect(child, savePart);
                }

            }

            //헤어
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.hairObject != null)
                {
                    savePart = partSelectionController.characterParts.hairObject;
                    SaveOjbect(child, savePart);
                }
            }

            //머리 악세서리
            if (child.gameObject.name == "Head_Parent")
            {
                if (partSelectionController.characterParts.headAccesoriesObject != null)
                {
                    savePart = partSelectionController.characterParts.headAccesoriesObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 숄더
            if (child.gameObject.name == "ShoulderR_Parent")
            {
                if (partSelectionController.characterParts.rightShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.rightShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 엘보
            if (child.gameObject.name == "ElbowR_Parent")
            {
                if (partSelectionController.characterParts.rightElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.rightElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무기
            if (child.gameObject.name == "WeaponR_Parent")
            {
                if (partSelectionController.characterParts.rightWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.rightWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 어꺠
            if (child.gameObject.name == "ShoulderL_Parent")
            {
                if (partSelectionController.characterParts.leftShoulderObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShoulderObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 엘보
            if (child.gameObject.name == "ElbowL_Parent")
            {
                if (partSelectionController.characterParts.leftElbowObject != null)
                {
                    savePart = partSelectionController.characterParts.leftElbowObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무기
            if (child.gameObject.name == "WeaponL_Parent")
            {
                if (partSelectionController.characterParts.leftWeaponObject != null)
                {
                    savePart = partSelectionController.characterParts.leftWeaponObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 쉴드
            if (child.gameObject.name == "Shield_Parent")
            {
                if (partSelectionController.characterParts.leftShieldObject != null)
                {
                    savePart = partSelectionController.characterParts.leftShieldObject;
                    SaveOjbect(child, savePart);
                }
            }

            //가슴
            if (child.gameObject.name == "Chest_Parent")
            {
                if (partSelectionController.characterParts.chestObject != null)
                {
                    savePart = partSelectionController.characterParts.chestObject;
                    SaveOjbect(child, savePart);
                }
            }

            //스파인
            if (child.gameObject.name == "Spine_Parent")
            {
                if (partSelectionController.characterParts.spineObject != null)
                {
                    savePart = partSelectionController.characterParts.spineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //root
            if (child.gameObject.name == "Root_Parent")
            {
                if (partSelectionController.characterParts.lowerSpineObject != null)
                {
                    savePart = partSelectionController.characterParts.lowerSpineObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 힙
            if (child.gameObject.name == "HipR_Parent")
            {
                if (partSelectionController.characterParts.rightHipObject != null)
                {
                    savePart = partSelectionController.characterParts.rightHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //오른쪽 무릎
            if (child.gameObject.name == "KneeR_parent")
            {
                if (partSelectionController.characterParts.rightKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.rightKneeObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 힙
            if (child.gameObject.name == "HipL_Parent")
            {
                if (partSelectionController.characterParts.leftHipObject != null)
                {
                    savePart = partSelectionController.characterParts.leftHipObject;
                    SaveOjbect(child, savePart);
                }
            }

            //왼쪽 무릎
            if (child.gameObject.name == "KneeL_parent")
            {
                if (partSelectionController.characterParts.leftKneeObject != null)
                {
                    savePart = partSelectionController.characterParts.leftKneeObject;
                    SaveOjbect(child, savePart);
                }
            }
        }
        #endregion

        PrefabUtility.SaveAsPrefabAsset(enemyObj, prefabPath);
        PrefabUtility.UnloadPrefabContents(enemyObj);
    }


    bool CheckNumber()
    {
        if (saveIndexName.text == "")
        {
            Debug.Log("저장할 인덱스를 입력하세요");
            return false;
        }

        bool IsNotNumber(string s) => !double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        if (IsNotNumber(saveIndexName.text))
        {
            Debug.Log("숫자를 입력하세요");
            return false;
        }

        return true;
    }

    void SaveOjbect(Transform parent, GameObject saveObj)
    {
        GameObject go = Instantiate(saveObj);
        go.name = saveIndexName.text;
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        if(parent.gameObject.name == "ShoulderR_Parent" || parent.gameObject.name == "ElbowR_Parent")
        {
            Vector3 newScale = go.transform.localScale;
            newScale.x *= -1;
            go.transform.localScale = newScale;
        }

        go.SetActive(false);
    }
#endif
}
