// Copyright 2001-2016 Crytek GmbH / Crytek Group. All rights reserved.

using System;
using CryEngine.UI;
using CryEngine.UI.Components;

namespace CryEngine.Game
{
	/// <summary>
	/// Basic sample of running a Mono Application.
	/// </summary>
	public class Game : IGameUpdateReceiver, IDisposable
    {
		private static Game _instance;
        private Canvas _canvas;

        private DateTime _updateFPSTime = DateTime.MinValue;
		private int _frameCount = 0;
		private Text _fpsText;

        private Game()
        {
			// The server doesn't support rendering UI and receiving input, so initializing those system is not required.
			if(Engine.IsDedicatedServer)
			{
				return;
			}

            Input.OnKey += OnKey;
            //Only move to the map if we're not in the sandbox. The sandbox can open the map all by itself.
            if (!Engine.IsSandbox)
            {
                Engine.Console.ExecuteString("map example", false, true);
            }

            //Only Create UI elemtns after level has loaded, use OnLevelLoaded
            //CreateUI();
        }

        public static void Initialize()
		{
			if(_instance == null)
			{
				_instance = new Game();
			}
		}

        public static void OnLevelLoaded()
        {
            _instance?.CreateUI();
            GameFramework.RegisterForUpdate(_instance);
        }

        public static void Shutdown()
		{
            _instance?.Dispose();
			_instance = null;
		}

		private void CreateUI()
		{
			// Canvas to contain the FPS label.
			_canvas = SceneObject.Instantiate<Canvas>(null);

			// Create FPS Label.
			_fpsText = _canvas.AddComponent<Text>();
			_fpsText.Alignment = Alignment.TopLeft;
			_fpsText.Height = 28;
			_fpsText.Offset = new Point (20, 20);
			_fpsText.Color = Color.Yellow.WithAlpha(0.5f);
            _fpsText.Content = "FPS";
		}

        public virtual void OnUpdate()
        {
            // Update FPS Label.
            if (DateTime.Now > _updateFPSTime)
            {
                _fpsText.Content = _frameCount + " fps";
                _frameCount = 0;
                _updateFPSTime = DateTime.Now.AddSeconds(1);
            }
            _frameCount++;
        }

        private void OnKey(InputEvent e)
		{
			if(e.KeyPressed(KeyId.Escape))
			{
				Engine.Shutdown();
			}

            // Show/Hide FPS Label on F5.
            if (e.KeyPressed(KeyId.F5))
            {
                _fpsText.Active = !_fpsText.Active;
            }
        }

        public void Dispose()
        {
            if (Engine.IsDedicatedServer)
            {
                return;
            }

            Input.OnKey -= OnKey;
            _instance._canvas.Destroy();
            GameFramework.UnregisterFromUpdate(this);
        }
    }
}
