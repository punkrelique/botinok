module Botinok.Core

open Funogram.Types
open Funogram.Api

let processResultWithValue (result: Result<'a, ApiResponseError>) =
  match result with
  | Ok v -> Some v
  | Error e ->
    printfn "Server error: %s. Error code: %i" e.Description e.ErrorCode
    None

let processResult (result: Result<'a, ApiResponseError>) =
  processResultWithValue result |> ignore

let botResult config data =
  api config data |> Async.RunSynchronously
let bot config data = botResult config data |> processResult

