import React from "react";
import { Image } from "react-native";
import { PopButton } from "./PopButton";

export function MrPillLogo(scale?: number) {
  scale = scale || 1;
  const logo = <Image source={require("@/assets/images/icon.png")} style={{ alignSelf: "center", marginTop: 20 }}/>
  return <PopButton ButtonContent={logo} BackgroundColor="white" ButtonScale={scale? scale: 1}/>
}
