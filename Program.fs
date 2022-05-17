open System.IO
open System
open System.Text.Json
open TaoTeChing.Types
open TaoTeChing
open Giraffe.ViewEngine


let filePath = __SOURCE_DIRECTORY__ + "/TaoTeChing.txt"

// For more information see https://aka.ms/fsharp-console-apps
let file = File.ReadAllText(filePath)

let parseFile (text: string) (verseNumber: int): Chapter list =

    let rec divideIntoChapters (text:string) (verseNumber: int) (chaptersList: string list) : string list =
        let firstResult = text.IndexOf(string <| verseNumber + 1)

        match firstResult with
        | -1 -> text::chaptersList
        | _ ->
            let head = text.Substring(0, firstResult)
            let tail = text.Substring(firstResult)
            
            match String.IsNullOrWhiteSpace(head), String.IsNullOrWhiteSpace(tail) with
            | false, false-> 
                let newChapterList = head::chaptersList
                divideIntoChapters tail (verseNumber+1) newChapterList
            | _ -> chaptersList


    let result = divideIntoChapters text verseNumber []

    result
    |> List.map (fun r ->
         let id = r.Substring(0, r.IndexOf("\n")) |> int
         let text = r.Substring(r.IndexOf("\n"))
         {Chapter.Id = id; Chapter.Text = text.Trim()}
        )
    |> List.sortBy (fun c->c.Id)
    

let results = parseFile file 1

let json = JsonSerializer.Serialize(results)

File.WriteAllText(__SOURCE_DIRECTORY__ + "/TaoTeChing.json", json)

let htmlPage = Views.renderHome results |> RenderView.AsString.htmlDocument
File.WriteAllText(__SOURCE_DIRECTORY__ + "/site/index.html", htmlPage)

results
|> List.iter (fun x ->
        let htmlPage = Views.renderChapter x |> RenderView.AsString.htmlDocument
        Directory.CreateDirectory(__SOURCE_DIRECTORY__ + sprintf "/site/%i" x.Id) |> ignore
        File.WriteAllText(__SOURCE_DIRECTORY__ + sprintf "/site/%i/index.html" x.Id, htmlPage)
    )

if File.Exists(__SOURCE_DIRECTORY__ + "/site/styles.css") then
    File.Delete(__SOURCE_DIRECTORY__ + "/site/styles.css")

File.Copy(__SOURCE_DIRECTORY__ + "/static/styles.css", __SOURCE_DIRECTORY__ + "/site/styles.css")