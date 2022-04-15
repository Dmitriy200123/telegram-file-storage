import {editorSlice} from "../../../redux/editorSlice";
import React, {memo, useState} from "react";
import {Button} from "../../utils/Button/Button";
import "./LoadFile.scss"
import {NavLink} from "react-router-dom";
import {useAppDispatch} from "../../../utils/hooks/reduxHooks";

const {setFile} = editorSlice.actions;
export const LoadFile: React.FC<PropsType> = memo(({className}) => {
    const dispatch = useAppDispatch();
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
            <div className={"load__drop-area" + (drag ? " load__drop-area_hover" : "")}
                 onDragStart={(e) => dragStartHandler(e)}
                 onDragLeave={(e) => dragLeaveHandler(e)}
                 onDragOver={(e) => dragStartHandler(e)}
                 onDrop={(e) => onDropHandler(e)}>
                {drag ? <h3>Отпустите, чтобы загрузить</h3>
                    : <h3>Перетащите файл сюда или {" "}
                        <label className={"load__input-file"}>
                            загрузите вручную
                            <input type={"file"} onChange={(e) => onChangeInput(e)}/>
                        </label>
                    </h3>
                }
                <div>ИЛИ</div>
                <NavLink to={"custom"}><Button>Добавить ссылку или текст</Button></NavLink>
            </div>
        </div>

    );
})

type PropsType = { className?: string };


