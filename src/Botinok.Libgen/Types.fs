module Botinok.Libgen.Types

open System.Web

type Genre =
    | NonFiction
    | Fiction
    | ScientificArticles
    | Magazines
    
type DownloadType =
    | ResumedDllWithOriginalFilename // todo

type ResultPerPage =
    | TwentyFive =  25
    | Fifty = 50
    | OneHundred = 100
    
type SearchWithMask =
    | Yes
    | No

type SearchInFields =
    | Title
    | Author
    | Series
    | Publisher
    | Year
    | ISBN
    | Language
    | MD5
    | Tаgs
    | Extensions
    
[<RequireQualifiedAccess>]
type Extension =
    | pdf
    | chm
    | djvu
    | rar
    | epub
    | azw3
    | undefined

type SearchQuery =
    { Req     : string
      LgTopic : string
      Open    : int
      View    : string
      Res     : int
      Phrase  : int
      Column  : string
      Page    : int }
    static member createDefaultQuery(book, page) = { Req = book; Page = page; Column = "def"; Phrase = 1; Open = 0; Res = 5; View = "simple"; LgTopic = "libgen" }
    member this.asQueryParams () =
        "?"
        +
        (
             [
                "req", this.Req
                "lg_topic", this.LgTopic
                "open", string this.Open
                "view", this.View
                "res", string this.Res
                "phrase", string this.Phrase
                "column", this.Column
                "page", string this.Page
            ]
            |> List.map (fun (k, v) -> sprintf "%s=%s" (HttpUtility.UrlEncode k) (HttpUtility.UrlEncode v))
            |> String.concat("&")
        ) 

type Book =
    { Id         : string
      Authors    : string list
      Name       : string
      ISBN       : string
      Edition    : string
      Publisher  : string
      Year       : string
      Pages      : string
      Language   : string
      Size       : string
      Extension  : string }
    override this.ToString () = $"""
Name: {this.Name}
Language: {this.Language}
Extension: {this.Extension}
Size: {this.Size}
Year: {this.Year}
Authors: {this.Authors |> String.concat " "}
Edition: {this.Edition}
Pages: {this.Pages}
Publisher: {this.Publisher}
ISBN: {this.ISBN}
Id: {this.Id}
"""