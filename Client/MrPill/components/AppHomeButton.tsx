import React from 'react';
import {View, StyleSheet, Pressable, Text} from 'react-native';
import { PopButton } from './PopButton';

type AppHomeButtonProps = {
  ButtonContent: React.JSX.Element | React.JSX.Element[];
  ButtonAction?: Function;
  BackgroundColor?: string;
  BorderColor?: string;
  BorderWidth?: number;
  Type?: number;
};

const emojis = ["", "🗄", "💊", "⏰", "➕",];

export const AppHomeButton: React.FC<AppHomeButtonProps> = ({ ButtonContent, ButtonAction, BackgroundColor, BorderColor, BorderWidth, Type }) => {

  if (!BackgroundColor) BackgroundColor = "#c9c9ff";
  if (!BorderColor) BorderColor = "#8a8aa7";
  if (!BorderWidth) BorderWidth = 3;

  const handleOnPress = () => {

    ButtonAction? ButtonAction() : {};
    
  };

  return (
    <PopButton DisableRotation={true} ButtonContent={
        <View style={[style.button, {backgroundColor: BackgroundColor, borderColor: BorderColor, borderWidth: BorderWidth}]}>
            {Type && <Text style={{fontSize: 72, position: 'absolute', color: "#FFFFFF88"}}>{emojis[Type]}</Text>}
            {ButtonContent}
        </View>
    } ButtonAction={handleOnPress}>
    </PopButton>
  );
};

const style = StyleSheet.create({  
  button: {
    minWidth: 150,
    maxHeight: 150,
    flexGrow: 1,
    borderRadius: 1000,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
    elevation: 5,
    shadowRadius: 15
  },
});