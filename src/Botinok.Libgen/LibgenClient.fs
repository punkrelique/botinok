module Botinok.LibgenClient

open System
open System.Net.Http
open System.Threading.Tasks
open Botinok.Common
open Botinok.Libgen.Parser
open Botinok.Libgen.Types

[<Literal>]
let linksPageUrl = "http://library.lol/main/"

[<Literal>]
let baseAddress = "https://www.libgen.is/"

type LibgenClient () =
    let httpClient = new HttpClient()
    member this.Search (query: SearchQuery) = task {
        try
            let! response = httpClient.GetAsync(baseAddress + "search.php" + query.asQueryParams())
            let! content = response.Content.ReadAsStringAsync()
            let books = extractBooks content
            
            $"Books search request: {Environment.NewLine} %A{query}" |> logInfo
            
            return Result.Ok books
            
        with
        | e ->
            $"Error searching books: {e.Message}" |> logErr
            return Result.Error "либген просит попожы"
    }

    member this.GetBook (bookId: string) = task {
        try
            let! response = httpClient.GetAsync(baseAddress + $"book/index.php?md5={bookId}")
            let! content = response.Content.ReadAsStringAsync()
            let book = extractBook content
            
            $"Book info request: %A{book}" |> logInfo
            
            return Result.Ok book
        with
        | e ->
           $"Error getting book: {e.Message}" |> logErr
           return Result.Error "ошибочкаАААААААААА"
    }

    member this.DownloadBook (bookId: string) = task {
        try
            let! response = httpClient.GetAsync(linksPageUrl + bookId) |> Async.AwaitTask
            let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
            let links = extractLinksFromDownloadPage content |> Seq.toArray
            let! tasks =
                links
                |> Seq.map httpClient.GetStreamAsync
                |> Task.WhenAny
                |> Async.AwaitTask

            $"Book download request: %A{bookId}" |> logInfo
            
            use! document = tasks |> Async.AwaitTask
            let contentType = links[0].Split '.' |> Seq.last

            return Result.Ok (document, $"{bookId}.{contentType}")
        with
        | e ->
           $"Error downloading  book: {e.Message}" |> logErr
           return Result.Error "не получилось скачать)"
    }
