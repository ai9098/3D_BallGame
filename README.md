# 3D_BallGame
Unity・C#を用いて作成した3Dのボールゲームです。
移動やジャンプでコインを集めてクリアフラグを出し、フラグに触れるとステージクリアです。

![demo](./Image/BallGame.gif)

## Movie
[https://youtu.be/cTI-7u9GDuI](https://youtu.be/cTI-7u9GDuI)

## Features
- 移動やジャンプでコインを集める。
- コインをすべて集めるとクリアフラグが出現し、触れるとクリア。
- ジャンプに違和感がないように、地面からの高さを計算。
- ボードの下に透明な判定オブジェクトがあり、ボードから落下して判定に触れた瞬間ステージが再読み読みされる。

## Technologies Used
- Unity
- C#

## Folder Structure
'''text
BallGame/
├── Assets/
│   ├── Scripts/
│   │   ├── FollowPlayer.cs  # カメラのスクリプト
│   │   ├── PlayerManager.cs # プレイヤーのスクリプト
│   │   └── RotateCoin.cs    # コインのスクリプト
│   ├── Scenes/           # ゲームのステージファイル
│   └── Material/         # Material関連のアセット
├── Build/                # Windows向け実行ファイル（ここからゲームをプレイできます）
└── README.md
'''
