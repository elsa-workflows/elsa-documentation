import { useId } from 'react'
import clsx from 'clsx'

import { InstallationIcon } from '@/components/icons/InstallationIcon'
import { LightbulbIcon } from '@/components/icons/LightbulbIcon'
import { PluginsIcon } from '@/components/icons/PluginsIcon'
import { PresetsIcon } from '@/components/icons/PresetsIcon'
import { ThemingIcon } from '@/components/icons/ThemingIcon'
import { WarningIcon } from '@/components/icons/WarningIcon'

const icons = {
  installation: InstallationIcon,
  presets: PresetsIcon,
  plugins: PluginsIcon,
  theming: ThemingIcon,
  lightbulb: LightbulbIcon,
  warning: WarningIcon,
}

const iconStyles = {
  green: '[--icon-foreground:theme(colors.slate.900)] [--icon-background:theme(colors.white)]',
  lime:
    '[--icon-foreground:theme(colors.lime.900)] [--icon-background:theme(colors.lime.100)]',
}

export function Icon({ color = 'green', icon, className, ...props }) {
  let id = useId()
  let IconComponent = icons[icon]

  return (
    <svg
      aria-hidden="true"
      viewBox="0 0 32 32"
      fill="none"
      className={clsx(className, iconStyles[color])}
      {...props}
    >
      <IconComponent id={id} color={color} />
    </svg>
  )
}

const gradients = {
  green: [
    { stopColor: '#bbf7d0' },
    { stopColor: '#4ade80', offset: '.527' },
    { stopColor: '#15803d', offset: 1 },
  ],
  lime: [
    { stopColor: '#ecfccb', offset: '.08' },
    { stopColor: '#84cc16', offset: '.837' },
  ],
}

export function Gradient({ color = 'green', ...props }) {
  return (
    <radialGradient
      cx={0}
      cy={0}
      r={1}
      gradientUnits="userSpaceOnUse"
      {...props}
    >
      {gradients[color].map((stop, stopIndex) => (
        <stop key={stopIndex} {...stop} />
      ))}
    </radialGradient>
  )
}

export function LightMode({ className, ...props }) {
  return <g className={clsx('dark:hidden', className)} {...props} />
}

export function DarkMode({ className, ...props }) {
  return <g className={clsx('hidden dark:inline', className)} {...props} />
}
