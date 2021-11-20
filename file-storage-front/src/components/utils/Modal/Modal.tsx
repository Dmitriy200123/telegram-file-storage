import React from 'react';
import "./Modal.scss"

const Modal:React.FC = ({children}) => {
    return (
        <div className={"modal"}>
            <div className={"modal__content"}>
                {children}
            </div>
        </div>
    )
}

export default Modal;
