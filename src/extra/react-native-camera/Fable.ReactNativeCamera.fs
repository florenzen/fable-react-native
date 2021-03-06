namespace Fable.ReactNativeCamera

open Fable.Core
open Fable.Core.JsInterop
open Fable.React

module Props =

    [<RequireQualifiedAccess>]
    type CameraStyle =
        | Flex of float

    type ICameraProperties =
        interface end

    [<RequireQualifiedAccess>]
    type CameraProperties =
        interface ICameraProperties
        static member Style (style: CameraStyle list) : ICameraProperties = !!("style", keyValueList CaseRules.LowerFirst style)

[<AutoOpen>]
module Helpers =

    open Props

    type Barcode =
        abstract data: string with get, set
        abstract ``type``: string with get, set

    type CameraRef() =
        let cameraRef = ref None

        /// Sets the internal ref
        member internal __.SetRef (ref: obj) : unit = 
            cameraRef := Some ref


        /// Take a picture
        member __.Capture() = 
            match !cameraRef with
            | Some v -> 
                promise {
                    let! data = unbox(v?capture())
                    return data
                }
            | _ -> promise { return! failwith "Camera not initialized!" }


    // Allows barcode reading
    let barcodeScanner (props:ICameraProperties seq) (ref:CameraRef) (onBarcodeRead: Barcode -> unit) (children: ReactElement seq) : ReactElement =
        let additionalProps =
            createObj [
                "onBarCodeRead" ==> onBarcodeRead 
                "ref" ==> ref.SetRef]

        let props =
            !!JS.Object.assign(
                additionalProps,
                keyValueList CaseRules.LowerFirst props)

        ofImport "RNCamera" "react-native-camera" props children
