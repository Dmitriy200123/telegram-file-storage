import React, {memo} from "react";
import {Dispatch} from "@reduxjs/toolkit";
import "./PreviewFile.scss"
import {ReactComponent as File} from "../../../assets/file.svg";
import {editorSlice} from "../../../redux/editorSlice";
import {Button} from "../../utils/Button/Button";
import {postFile} from "../../../redux/thunks/fileThunks";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";

const {setFile} = editorSlice.actions;
export const PreviewFile: React.FC<PropsType> = memo(({
                                                          className,
                                                          file
                                                      }) => {
    const dispatch = useAppDispatch();

    function setFileNull() {
        dispatch(setFile(null))
    }

    function post() {
        const formData = new FormData();
        formData.append("file", file);
        dispatch(postFile(formData))
    }

    return (
        <div className={className + "  preview-file"}>
            <div className={"preview-file__content"}>
                <div className={"preview-file__file"}>
                    <File className={"preview-file__svg"}/>
                    <div className={"preview-file__file-load"}>
                        <p className={"preview-file__name"}>{file.name}</p>
                        <button className="preview-file__close" onClick={setFileNull}>1</button>
                        {/*<div className={"preview-file__loading"}/>*/}
                    </div>
                </div>
                <div className={"preview-file__btns"}>
                    <Button onClick={post}>Загрузить</Button>
                    <Button type={"transparent"} onClick={setFileNull}>Отмена</Button>
                </div>
            </div>
        </div>

    );
})


type PropsType = { className?: string, file: File };



