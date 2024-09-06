import React from "react";
import { Image } from "react-native";
import { PopButton } from "./PopButton";
import { router } from "expo-router";

export function MrPillLogo(scale?: number, home?: boolean, position?: "absolute" | "relative" | "static") {
  scale = scale || 1;
  const logo = <Image source={require("@/assets/images/icon.png")} style={{ alignSelf: "center" }}/>
  return <PopButton ButtonContent={logo} BackgroundColor="white" ButtonScale={scale? scale: 1} Position={position} ButtonAction={()=>{if (!home) router.back()}}/>
}
