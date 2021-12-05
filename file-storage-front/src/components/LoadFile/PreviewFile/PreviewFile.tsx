import React, {memo} from "react";
import {Dispatch} from "@reduxjs/toolkit";
import "./PreviewFile.scss"
import {ReactComponent as File} from "../../../assets/file.svg";
import {editorSlice} from "../../../redux/editorSlice";
import {Button} from "../../utils/Button/Button";

const {setFile} = editorSlice.actions;
export const PreviewFile: React.FC<{ dispatch: Dispatch, className?: string, file: File }> = memo(({
                                                                                                       dispatch,
                                                                                                       className,
                                                                                                       file
                                                                                                   }) => {
    function setFileNull() {
        dispatch(setFile(null))
    }

    return (
        <div className={className + "  preview-file"}>
            <div className={"preview-file__content"}>
                <div className={"preview-file__file"}>
                    <File className={"preview-file__svg"}/>
                    <p className={"preview-file__name"}>{file.name}</p>
                    <button className="preview-file__close" onClick={setFileNull}>1
                    </button>
                </div>
                <div className={"preview-file__btns"}>
                    <Button>Загрузить</Button>
                    <Button type={"transparent"} onClick={setFileNull}>Отмена</Button>
                </div>
            </div>
        </div>

    );
})




