# 合成音声動画を F# スクリプトから生成する(WIP)

本リポジトリは現在**作業中**の仮のものとなります．

## インストール手順

1. `git clone`
2. [.NET](https://dotnet.microsoft.com/en-us/) 環境を整える(≥ 7.0.102)
3. [FFmpeg](https://ffmpeg.org/)，[ImageMagick](https://imagemagick.org/index.php)，[Typst](https://github.com/typst/) をインストール
4. `commandConfig.json` ファイルにて適切なパスを指定
5. [Voicevox Core](https://github.com/VOICEVOX/voicevox_core) の Release を入手し，`model`` フォルダを `VoicevoxCoreWrapper/bin` フォルダ内に，`open_jtalk_dic_utf_8-*` フォルダを `voicevox_core` フォルダ内に配置

## 使い方

`src/App/Program.fs` を編集し，`dotnet run` で実行
