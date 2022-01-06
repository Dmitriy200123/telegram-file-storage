import "./Profile.scss";
import React, {FC, memo} from "react";
import {ReactComponent as Telegram} from "./../../assets/telegram.svg";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {fetchAuthGitlab} from "../../redux/thunks/profileThunks";

export const Profile: FC = memo(() => {
    const {name} = useAppSelector((state) => state.profile);
    function onAuth(){
        let dataAuthGit = localStorage.getItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
        if (!dataAuthGit)
            dataAuthGit = sessionStorage.getItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
        if (dataAuthGit) {
            const json = JSON.parse(dataAuthGit);
            window.open(`https://t.me/sixty_six_bit_bot?start=${json?.access_token}`);
        }


    }
    return (<div className={"profile"}>
            <div className={"profile__block-img"}>
                <div className={"profile__img"}>
                 <img/>
                </div>
                <span onClick={onAuth} className={"profile__telegram"}><span>Подключить телеграмм</span> <Telegram/></span>
            </div>
            <div className={"profile__info"}>
                <h2 className={"profile__h2"}>Профиль</h2>
                <h3 className={"profile__h3"}>Имя</h3>
                <p className={"profile__p"}>{name}</p>
            </div>
        </div>
    );
})




