open Botinok.Common
open Funogram
open Funogram.Telegram.Bot
open Botinok
open Funogram.Api
open Funogram.Telegram
open Microsoft.Extensions.Configuration
open Serilog

[<EntryPoint>]
let main _ =
    async {
        Log.logger <-
            LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Information()
                .CreateLogger()

        let appConfig =
            ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json")
                .Build()

        let botConfig =
            { Config.defaultConfig with
                Token = appConfig["Botinok:Token"] }

        botConfig
        |> sprintf "Bot configuration: %A"
        |> logInfo

        let! _ = Api.deleteWebhookBase () |> api botConfig
        return! startBot botConfig Commands.Base.updateArrived None
    }
    |> Async.RunSynchronously

    0