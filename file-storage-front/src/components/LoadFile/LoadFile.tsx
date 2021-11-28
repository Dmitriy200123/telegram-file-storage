import React, {memo, useState} from 'react';
import "./LoadFile.scss"
import {Button, InputFile} from "../utils/Button/Button";

export const LoadFile: React.FC<any> = memo(({match}) => {
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
        // @ts-ignore
        const files = [...e.dataTransfer.files];
        const formData = new FormData();
        formData.append("file", files[0]);
        setDrag(false);
        console.log(files)
    }

    return (
        <div className={"load-file"}>
            <h2 className={"load-file__h2"}>Загрузка файлов</h2>
            <div className={"load-file__content"}>
                <div className={"load-file__drop-area"} onDragStart={(e) => dragStartHandler(e)} onDragLeave={(e) => dragLeaveHandler(e)}
                onDragOver={(e)=> dragStartHandler(e)}
                onDrop={(e) => onDropHandler(e)}>
                    {drag ? <h3>Отпустите, чтобы загрузить</h3> : <h3>Перетащите файл сюда или загрузите вручную</h3>}
                    <div>ИЛИ</div>
                    <InputFile className={"load-file__btn-load"}></InputFile>
                </div>
            </div>
        </div>
    );
})




