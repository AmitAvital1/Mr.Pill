import React from 'react';
import {View, StyleSheet, Pressable, Text} from 'react-native';

type AppTextButtonProps = {
  ButtonContent: string;
  ButtonAction?: Function;
  BackgroundColor?: string;
};

export const AppTextButton: React.FC<AppTextButtonProps> = ({ ButtonContent, ButtonAction }) => {


  const handleOnPress = () => {

    ButtonAction? ButtonAction() : {};
    
  };

  return (
    <View style={style.button}>
      <Pressable onPress={handleOnPress}>
          <Text style={style.buttontext}>{ButtonContent}</Text>
      </Pressable>
    </View>
  );
};

const style = StyleSheet.create({  
  button: {
    minHeight: 100,
    backgroundColor: "#6666FC",
    flex: 1,
    borderRadius: 30,
    justifyContent: "center",
    alignItems: "center",
    margin: 10,
    borderColor: "black",
  },
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 20,
  },
  buttontext: {
    fontSize: 16,
  },
});