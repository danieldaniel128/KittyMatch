using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    public void WinEvent()
    {
        //win logic
    }
    public void LoseEvent()
    {
        //lose logic
    }
    [ContextMenu("ResetGame")]
    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
