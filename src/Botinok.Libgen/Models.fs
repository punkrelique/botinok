module Botinok.Libgen.Models

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