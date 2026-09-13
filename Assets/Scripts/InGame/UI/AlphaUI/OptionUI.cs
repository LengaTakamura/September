using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using September.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace InGame.UI
{
    public class OptionUI : MonoBehaviour
    {
        [SerializeField, Label("表示非表示させるUI")] private CanvasGroup _optionUIPanel;
        [SerializeField, Label("表示時に選択するUI")] private Selectable _selectWhenOpen;

        private GameInput _gameInput;
        private bool _isShow;

        private HashSet<GameObject> _childSelectables;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _childSelectables = GetComponentsInChildren<Selectable>().Select(x => x.gameObject).ToHashSet();
            _gameInput = GameInput.I;
            Show(false);
        }

        private void Update()
        {
            // オプションUIの表示切り替え
            if (_gameInput.UI.Option.triggered)
            {
                Show(!_isShow);
            }

            // オプション画面等を開いていて、かつマウスを使用している場合はカーソルを表示する
            // 現状この機能を使うのがInGameSceneのみかつ、OptionUIが存在するのもInGameSceneのみなので動作上問題ない
            // 他の場所でも使うようにするなら別の所で処理するようにする
            // ↑特定のシーン上でしか動作させなくていいことに変わりはないと思うので、そこをどう制御するかは必要な時に考える
            CursorStateManager.UpdateCursorState();
        }

        public void Show(bool isShow)
        {
            _isShow = isShow;

            if (_optionUIPanel)
            {
                _optionUIPanel.alpha = isShow ? 1 : 0;
                _optionUIPanel.interactable = isShow;
                _optionUIPanel.blocksRaycasts = isShow;
            }
            
            if (_gameInput != null)
                _gameInput.IsInputBlockedByUI = _isShow;

            if (_isShow)
            {
                if (_optionUIPanel)
                    _optionUIPanel.transform.SetAsLastSibling();
                if (_selectWhenOpen)
                    EventSystem.current.SetSelectedGameObject(_selectWhenOpen.gameObject);
            }

            if (_isShow)
            {
                CursorStateManager.ShowCursor();
            }
            else
            {
                CursorStateManager.HideCursor();
            }

            if (!_isShow && _childSelectables.Contains(EventSystem.current.currentSelectedGameObject))
            {
                // 非アクティブにするだけだと選択解除されないので明示的に行う
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        private void OnDestroy()
        {
            if (!_isShow) return;

            CursorStateManager.HideCursor();
            if (_gameInput != null) _gameInput.IsInputBlockedByUI = false;
        }
    }
}
