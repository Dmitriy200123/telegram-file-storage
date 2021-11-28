import React, {memo, useState} from 'react';
import "./LoadFile.scss"

export const LoadFile:React.FC<any> = memo(({match}) => {
    const [drag, setDrag] = useState(false);
    return (
        <div className={"load-file"}>
            <h2 className={"load-file__h2"}>Загрузка файлов</h2>
            <div className={"load-file__content"}>
                <div className={"load-file__drag"}>
                    <h3>Перетащите файл сюда или загрузите вручную</h3>
                </div>
            </div>
        </div>
    );
})




