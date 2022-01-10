import {editorSlice} from "../../../redux/editorSlice";
import React, {memo, useState} from "react";
import {Button} from "../../utils/Button/Button";
import {Dispatch} from "@reduxjs/toolkit";
import "./LoadFile.scss"
import { Link, NavLink } from "react-router-dom";

const {setFile} = editorSlice.actions;
export const LoadFile: React.FC<{ dispatch: Dispatch, className?:string }> = memo(({dispatch, className}) => {
    const [drag, setDrag] = useState(false);

    function dragStartHandler(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
        setDrag(true);
    }

    function dragLeaveHandler(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
        setDrag(false)
    }

    function onDropHandler(e: React.DragEvent<HTMLDivElement>) {
        e.preventDefault();
        const file = e.dataTransfer.files && e.dataTransfer.files[0];
        dispatch(setFile(file));
        setDrag(false);
    }

    function onChangeInput(e: React.ChangeEvent<HTMLInputElement>) {
        const file = e.target.files && e.target.files[0];
        if (file)
            dispatch(setFile(file));
    }

    return (
        <div className={className + "  load"}>
            <div className={"load__drop-area"} onDragStart={(e) => dragStartHandler(e)}
                 onDragLeave={(e) => dragLeaveHandler(e)}
                 onDragOver={(e) => dragStartHandler(e)}
                 onDrop={(e) => onDropHandler(e)}>
                {drag ? <h3>Отпустите, чтобы загрузить</h3>
                    :
                    <h3>Перетащите файл сюда или {" "}
                        <label className={"load__input-file"}>
                            загрузите вручную <input type={"file"} onChange={(e) => onChangeInput(e)}/>
                        </label>
                    </h3>
                }
                <div>ИЛИ</div>
                <NavLink to={"ВСТАВЬ-СЮДА-УРЛ"} ><Button>Добавить ссылку или текст</Button></NavLink>
            </div>
        </div>

    );
})




