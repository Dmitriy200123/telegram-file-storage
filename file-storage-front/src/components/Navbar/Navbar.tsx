import "./Navbar.scss";
import React, {FC, memo} from "react";
import {Link} from "react-router-dom";
import {ReactComponent as Logo} from "./../../assets/logo.svg";
import {ReactComponent as Download} from "./../../assets/download_2.svg";
import {ReactComponent as Search} from "./../../assets/search.svg";
import {Button} from "../utils/Button/Button";
import {useAppDispatch} from "../../utils/hooks/reduxHooks";
import {fetchLogout} from "../../redux/profileThunks";


export const Navbar: FC<{ className?: string }> = memo(({className}) => {
    const dispatch = useAppDispatch();
    return (<div className={(`${className} ` ?? "") + "navbar"}>
            <div className={"navbar__logo"}>
                <Logo/>
            </div>
            <div className={"navbar__links"}>
                <Link to={"/load/"} className={"navbar__link"}><Download/><span>Загрузка файлов</span></Link>
                <Link to={"/files"} className={"navbar__link"}><Search/>Поиск файлов</Link>
                <button className={"navbar__link"} onClick={() => dispatch(fetchLogout())}>Выйти из аккаунта</button>
            </div>
        </div>
    );
})




