module Botinok.Libgen.Parser

open FSharp.Data
open Botinok.Libgen.Types

[<Literal>]
let private newLine = "\r\n"

let private getId (nodes: HtmlNode) =
    nodes.TryGetAttribute("href")
    |> Option.map (fun x -> x.Value())
    |> Option.defaultValue ""
    |> fun x -> x.Split('=') |> Seq.last

let extractBooks content =
    let table =
        HtmlDocument.Parse(content).Body().Descendants["table"]
        |> Seq.skip 2
        |> Seq.head

    table.Descendants["tr"]
    |> Seq.skip 1
    |> Seq.map (fun x -> x.Elements() |> Seq.toList)
    |> Seq.map (fun x ->
        let name =
            x[2].Descendants["a"]
            |> Seq.head
            |> fun x -> x.Elements() |> Seq.head |> (fun x -> x.InnerText())

        let isbn = x[2].Descendants() |> Seq.last |> (fun x -> x.InnerText())

        let edition =
            x[2].Descendants["font"]
            |> Seq.toList
            |> fun nodes -> if nodes.Length = 3 then nodes |> List.skip 1 else nodes
            |> List.tryHead
            |> Option.map (fun node -> node.InnerText())
            |> Option.defaultValue ""
            |> fun text -> if (text = isbn || text = name) then "" else text

        { ISBN = isbn
          Authors =
            x[1].Descendants()
            |> Seq.toList
            |> List.filter (fun node ->
                match node with
                | HtmlText text -> text <> newLine
                | _ -> false)
            |> List.map (fun x -> x.InnerText())

          Name = name
          Publisher = x[3].InnerText()
          Edition = edition
          Year = x[4].InnerText()
          Pages = x[5].InnerText()
          Language = x[6].InnerText()
          Size = x[7].InnerText()
          Extension = x[8].InnerText()
          Id = x[2].Descendants["a"] |> Seq.last |> getId })

let extractLinksFromDownloadPage content =
    HtmlDocument.Parse(content).Descendants["a"]
    |> Seq.choose (fun x -> x.TryGetAttribute("href") |> Option.map (fun a -> a.Value()))
    |> Seq.take 4
    
let extractBook content =
    let trs =
        HtmlDocument.Parse(content).Descendants["table"]
        |> Seq.head
        |> fun x -> x.Elements()

    { ISBN = (trs[7].Elements()[1]).InnerText()
      Authors = (trs[2].Elements()[1]).InnerText().Split(',') |> Seq.toList
      Name = (trs[1].Elements()[2]).InnerText()
      Publisher = (trs[4].Elements()[1]).InnerText()
      Edition = (trs[5].Elements()[3]).InnerText()
      Year = (trs[5].Elements()[1]).InnerText()
      Pages = (trs[6].Elements()[3]).InnerText()
      Language = (trs[6].Elements()[1]).InnerText()
      Size = (trs[10].Elements()[1]).InnerText().Split(' ') |> Seq.take 2 |> String.concat ""
      Extension = (trs[10].Elements()[3]).InnerText()
      Id = trs[11].Descendants["a"] |> Seq.head |> getId }
    
    