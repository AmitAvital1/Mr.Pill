import React, { useEffect, useRef } from 'react';
import {View, StyleSheet, Pressable, Animated} from 'react-native';


type PopButtonProps = {
  ButtonContent: React.JSX.Element | React.JSX.Element[];
  ButtonAction?: Function;
  BackgroundColor?: string;
  ButtonScale?: number;
  Position?: "absolute" | "relative" | "static";
};

export const PopButton: React.FC<PopButtonProps> = ({ ButtonContent, ButtonAction, ButtonScale, Position }) => {
  const animatedScale = useRef(new Animated.Value(0)).current;
  const animatedRotation = useRef(new Animated.Value(0)).current;

  if (!ButtonScale) {
    ButtonScale = 1
  }

  if (!Position) {
    Position = "relative";
  }

  useEffect(() => {
    animatedScale.setValue(1 * ButtonScale);
  }, []);

  const handleOnPress = () => {
    animatedScale.setValue(0.8 * ButtonScale);
    animatedRotation.setValue(0);

    Animated.parallel([
      Animated.spring(animatedScale, {
        toValue: 1 * ButtonScale,
        bounciness: 24,
        speed: 20,
        useNativeDriver: true,
      }),
      Animated.timing(animatedRotation, {
        toValue: Math.random() * 0.02 * (Math.random() < 0.5 ? -1: 1),
        duration: 400,
        useNativeDriver: true,
      }),
    ]).start();

    ButtonAction ? ButtonAction() : {};
  };

  const rotation = animatedRotation.interpolate({
    inputRange: [0, 1],
    outputRange: ['0deg', '360deg'], // Rotate from 0 to 360 degrees
  });


  return (
    <View style={[style.container, {maxHeight: 180 * ButtonScale, position: Position}]}>
      <Pressable onPress={handleOnPress}>
        <Animated.View
          style={[
            style.button,
            { transform: [{ scale: animatedScale }, { rotate: rotation }] },
          ]}>
          {ButtonContent}
        </Animated.View>
      </Pressable>
    </View>
  );
}

const style = StyleSheet.create({
  container: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    margin: 8,
  },
  button: {
    justifyContent: 'center',
    alignItems: 'center',
  },
});