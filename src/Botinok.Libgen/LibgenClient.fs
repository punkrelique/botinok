module Botinok.LibgenClient

open System
open System.Net.Http
open System.Threading.Tasks
open System.Web
open Botinok.Common
open Botinok.Libgen.Parser

[<Literal>]
let linksPageUrl = "http://library.lol/main/"

[<Literal>]
let baseAddress = "https://www.libgen.is/"

type SearchQuery = {
    Req: string
    LgTopic: string
    Open: int
    View: string
    Res: int
    Phrase: int
    Column: string
    Page: int
}

type LibgenClient () =
    let httpClient = new HttpClient()
    let buildSearchQuery (query: SearchQuery) =
        "?"
        +
        (
             [
                "req", query.Req
                "lg_topic", query.LgTopic
                "open", string query.Open
                "view", query.View
                "res", string query.Res
                "phrase", string query.Phrase
                "column", query.Column
                "page", string query.Page
            ]
            |> List.map (fun (k, v) -> sprintf "%s=%s" (HttpUtility.UrlEncode k) (HttpUtility.UrlEncode v))
            |> String.concat("&")
        ) 
        
    member this.Search (query: SearchQuery) = task {
        try
            let! response = httpClient.GetAsync(baseAddress + "search.php" + buildSearchQuery query)
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
            
            let! document = tasks |> Async.AwaitTask
            let contentType = links[0].Split '.' |> Seq.last

            return Result.Ok (document, $"{bookId}.{contentType}")
        with
        | e ->
           $"Error downloading  book: {e.Message}" |> logErr
           return Result.Error "не получилось скачать)"
    }
