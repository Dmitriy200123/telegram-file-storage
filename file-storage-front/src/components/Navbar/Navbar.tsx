import "./Navbar.scss";
import React, {FC, memo} from "react";
import {Link} from "react-router-dom";
import {ReactComponent as Logo} from "./../../assets/logo.svg";

export const Navbar: FC<{ className?: string }> = memo(({className}) => {
    return (<div className={(`${className} ` ?? "") + "navbar"}>
            <div className={"navbar__logo"}>
                <Logo/>
            </div>
            <div style={{display: "grid", gap: 10}}>
                <Link to={"/load/"}>Загрузить файл</Link>
                <Link to={"/files"}>Искать файлы</Link>
                <Link to={"/login"}>логин</Link>
            </div>
        </div>
    );
})




