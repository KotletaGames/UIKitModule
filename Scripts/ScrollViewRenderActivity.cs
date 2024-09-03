using Cysharp.Threading.Tasks;
using KotletaGames.AdditionalModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewRenderActivity : MonoBehaviour
{
    [SerializeField] private RectTransform[] _parents;
    [SerializeField] private RectTransform _main;

    private readonly List<RectCanvasRender> _rectCanvasRenders = new(16);

    private void Start()
    {
        foreach (var parent in _parents)
        {
            Graphic[] renderers = parent.GetComponentsInChildren<Graphic>();
            if (renderers.Length == 0)
                continue;

            RectCanvasRender rectCanvasRender = new(parent, renderers);
            _rectCanvasRenders.Add(rectCanvasRender);
        }

        _rectCanvasRenders.TrimExcess();

        Procces().Forget();
    }

    private async UniTaskVoid Procces()
    {
        while (destroyCancellationToken.IsCancellationRequested == false)
        {
            await UniTask.WaitWhile(() => gameObject.activeInHierarchy == false, cancellationToken: destroyCancellationToken);

            for (int i = 0; i < _rectCanvasRenders.Count; i++)
            {
                if (IsIntersecting(_main, _rectCanvasRenders[i].Parent) == true)
                    _rectCanvasRenders[i].Enable();
                else
                    _rectCanvasRenders[i].Disable();
            }

            await UniTask.Yield();
        }
    }

    private bool IsIntersecting(RectTransform main, RectTransform render)
    {
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];

        main.GetWorldCorners(cornersA);
        render.GetWorldCorners(cornersB);

        Rect rectA = new Rect(cornersA[0], new Vector2(cornersA[2].x - cornersA[0].x, cornersA[2].y - cornersA[0].y));
        Rect rectB = new Rect(cornersB[0], new Vector2(cornersB[2].x - cornersB[0].x, cornersB[2].y - cornersB[0].y));

        return rectA.Overlaps(rectB);
    }

    private struct RectCanvasRender
    {
        public readonly RectTransform Parent;
        private readonly Graphic[] _graphics;

        public RectCanvasRender(RectTransform parent, Graphic[] graphics)
        {
            Parent = parent;
            _graphics = graphics;

        }

        public void Enable()
        {

            for (int i = 0; i < _graphics.Length; i++)
                _graphics[i].ActiveSelf();

        }

        public void Disable()
        {

            for (int i = 0; i < _graphics.Length; i++)
                _graphics[i].DisactiveSelf();


            //_graphics[i].canvasRenderer.cull = true; // Отключает рендерин
        }
    }
}