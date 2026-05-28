using Firebase.Database;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    FirebaseDatabase database;
    DatabaseReference reference;
    UnityMainThreadDispatcher dispatcher;

    [SerializeField] InputField NickNameInput;
    [SerializeField] Text checkText;

    void Start()
    {
        database = FirebaseDatabase.GetInstance(
            "https://shingutest-2a317-default-rtdb.asia-southeast1.firebasedatabase.app/"
            );

        reference = database.RootReference;
        dispatcher = UnityMainThreadDispatcher.Instance();
    }

    public void OnClickRegister()
    {
        string nickName = NickNameInput.text.Trim();

        if(string.IsNullOrEmpty(nickName))
        {
            checkText.text = "닉네임을 입력하세요.";
            return;
        }

        reference.Child("UserInfo").OrderByChild("NickName").EqualTo(nickName).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                dispatcher.Enqueue(() =>
                {
                    checkText.text = "Firebase 읽기 오류";
                });
            }

            DataSnapshot snapshot = task.Result;

            if (snapshot.HasChildren)
            {
                dispatcher.Enqueue(() =>
                {
                    checkText.text = "이미 사용 중인 닉네임 입니다.";
                });
                return;
            }
            CreateUser(nickName);
        });
    }

    void CreateUser(string nickName)
    {
        DatabaseReference newUserRef = reference.Child("UserInfo").Push();

        string userKey = newUserRef.Key;

        UserData userData = new UserData(nickName);
        string json = JsonUtility.ToJson(userData);

        newUserRef.SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                dispatcher.Enqueue(() =>
                {
                    checkText.text = "회원 가입 실패";
                });
                return;
            }

            dispatcher.Enqueue(() =>
            {
                PlayerPrefs.SetString("UserKey", userKey);
                PlayerPrefs.SetString("UserNickName", nickName);
                PlayerPrefs.Save();

                checkText.text = "회원 가입 완료";
            });
        });
    }

}
